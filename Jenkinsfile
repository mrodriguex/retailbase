pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-u root:root -v /var/run/docker.sock:/var/run/docker.sock'
        }
    }
    
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
        DOTNET_CLI_HOME = '/tmp/dotnet-home'
    }

    stages {
        stage('Prepare') {
            steps {
                sh '''
                apt-get update
                apt-get install -y openssh-client
                mkdir -p /tmp/dotnet-home
                chmod 777 /tmp/dotnet-home
                '''
            }
        }

        stage('Checkout') {
            steps { 
                cleanWs()
                checkout scm 
                }
        }

         stage('Set Variables') {
            steps {
                script {
                    // Evaluar condicionales AQUÍ dentro de script
                    
                    env.USER = params.USER ?: 'mrodriguex'
                    env.SERVER = params.SERVER ?: 'localhost'
                    env.ENVIRONMENT = params.ENVIRONMENT ?: 'dev'
                    env.PROJECT_NAME = params.PROJECT_NAME ?: 'RETAIL.BASE.API'
                    env.SERVICE_PORT = params.SERVICE_PORT ?: '5000'                    
                    env.PROJECT_DIR = env.PROJECT_NAME

                    env.DEPLOY_PATH = params.DEPLOY_PATH ?: "/home/${env.USER}/www/services/${env.ENVIRONMENT}/${env.PROJECT_NAME}"
                    
                    env.SERVICE = "${env.PROJECT_NAME}-${env.ENVIRONMENT}.service"
                    
                    echo "Configuración:"
                    echo "  PROYECTO: ${env.PROJECT_NAME}"
                    echo "  AMBIENTE: ${env.ENVIRONMENT}"
                    echo "  SERVICIO: ${env.SERVICE}"
                    echo "  PATH: ${env.DEPLOY_PATH}"
                }
            }
        }

        stage('Build') {
            steps {
                dir("${env.PROJECT_DIR}") {
                    sh "dotnet restore ${env.PROJECT_NAME}.csproj"
                    sh "dotnet build ${env.PROJECT_NAME}.csproj -c Release"
                    sh "dotnet publish ${env.PROJECT_NAME}.csproj -c Release -o publish"
                    sh '''
                        echo "=== BUSCANDO appsettings ==="
                        find . -name "appsettings.json"
                    '''
                    sh "cat publish/appsettings.json"
                    echo "✅ Build completado para ${env.PROJECT_NAME} en ${env.ENVIRONMENT}"
                }
            }
        }

        stage('Deploy') {
            steps {
                sshagent(['server-deploy-key']) {

                    script {

                        def serviceContent = """\
[Unit]
Description=${env.PROJECT_NAME}
After=network.target

[Service]
WorkingDirectory=${env.DEPLOY_PATH}

ExecStart=${env.DEPLOY_PATH}/${env.PROJECT_NAME} --urls=http://0.0.0.0:${env.SERVICE_PORT}

User=${env.USER}
Group=${env.USER}

Restart=always
RestartSec=5

SyslogIdentifier=${env.PROJECT_NAME}-${env.ENVIRONMENT}

Environment=ASPNETCORE_ENVIRONMENT=${env.ENVIRONMENT}

PrivateTmp=true

[Install]
WantedBy=multi-user.target
"""

                        writeFile file: env.SERVICE, text: serviceContent

                        sh """
                            set -eux

                            echo "=== PREPARING REMOTE DIRECTORY ==="

                            ssh -o StrictHostKeyChecking=no \
                                ${env.USER}@${env.SERVER} "
                                    mkdir -p ${env.DEPLOY_PATH}
                                "

                            echo "=== INSTALLING SYSTEMD SERVICE ==="

                            scp -o StrictHostKeyChecking=no \
                                ${env.SERVICE} \
                                ${env.USER}@${env.SERVER}:/tmp/${env.SERVICE}

                            ssh -o StrictHostKeyChecking=no \
                                ${env.USER}@${env.SERVER} "
                                    sudo mv /tmp/${env.SERVICE} /etc/systemd/system/${env.SERVICE}
                                    sudo chmod 644 /etc/systemd/system/${env.SERVICE}
                                    sudo systemctl daemon-reload
                                    sudo systemctl enable ${env.SERVICE}
                                "

                            echo "=== CLEANING DEPLOY DIRECTORY ==="

                            ssh -o StrictHostKeyChecking=no \
                                ${env.USER}@${env.SERVER} "
                                    test -n "${env.DEPLOY_PATH}"
                                    test "${env.DEPLOY_PATH}" != "/"
                                    find "${env.DEPLOY_PATH}" -mindepth 1 -delete
                                "

                            echo "=== COPYING APPLICATION FILES ==="

                            scp -o StrictHostKeyChecking=no -r \
                                publish/* \
                                ${env.USER}@${env.SERVER}:${env.DEPLOY_PATH}/

                            echo "=== FIXING PERMISSIONS ==="

                            ssh -o StrictHostKeyChecking=no \
                                ${env.USER}@${env.SERVER} "
                                    chmod +x ${env.DEPLOY_PATH}/${env.PROJECT_NAME}
                                    chown -R ${env.USER}:${env.USER} ${env.DEPLOY_PATH}
                                "

                            echo "=== RESTARTING SERVICE ==="

                            ssh -o StrictHostKeyChecking=no \
                                ${env.USER}@${env.SERVER} "
                                    sudo systemctl restart ${env.SERVICE}

                                    sleep 3

                                    echo '=== SERVICE STATUS ==='

                                    sudo systemctl status ${env.SERVICE} \
                                        --no-pager \
                                        --full \
                                        | head -20
                                "

                            echo "✅ ${env.PROJECT_NAME} desplegado correctamente"
                        """
                    }
                }
            }
        }
    }

    post {
        success { 
            echo "✅ PIPELINE COMPLETADO - ${env.PROJECT_NAME} en ${env.ENVIRONMENT}"
        }
        failure { 
            echo "❌ FALLÓ - ${env.PROJECT_NAME} en ${env.ENVIRONMENT}"
        }
        always { 
            archiveArtifacts artifacts: '**/publish/**', allowEmptyArchive: true
        }
    }
}