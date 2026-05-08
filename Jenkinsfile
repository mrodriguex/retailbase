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
                    apt-get update && apt-get install -y openssh-client
                    mkdir -p /tmp/dotnet-home && chmod 777 /tmp/dotnet-home
                '''
            }
        }

        stage('Checkout') {
            steps { checkout scm }
        }

         stage('Set Variables') {
            steps {
                script {
                    // Evaluar condicionales AQUÍ dentro de script
                    
                    env.USER = params.USER ?: 'mrodriguex'
                    env.SERVER = params.SERVER ?: 'localhost'
                    env.ENVIRONMENT = params.ENVIRONMENT ?: 'dev'
                    env.PROJECT_NAME = params.PROJECT ?: 'RETAIL.BASE.API'
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
                    sh "dotnet publish ${env.PROJECT_NAME}.csproj -c Release -o ./publish"
                    sh '''
                        echo "=== BUSCANDO appsettings ==="
                        find . -name "appsettings.json"
                    '''
                    sh "cat ./publish/appsettings.json"
                    echo "✅ Build completado para ${env.PROJECT_NAME} en ${env.ENVIRONMENT}"
                }
            }
        }

        stage('Setup Service') {
            steps {
                sshagent(['server-deploy-key']) {
                    script {
                        def serviceContent = """\
[Unit]
Description=Servicio API de ${env.PROJECT_NAME}

[Service]
ExecStart=${env.DEPLOY_PATH}/${env.PROJECT_NAME} --urls http://0.0.0.0:${env.SERVICE_PORT}
WorkingDirectory=${env.DEPLOY_PATH}/
User=${env.USER}
Group=${env.USER}
Restart=on-failure
SyslogIdentifier=${env.PROJECT_NAME}-${env.ENVIRONMENT}
PrivateTmp=true
CPUWeight=20
CPUQuota=80%

[Install]
WantedBy=multi-user.target
"""
                        writeFile file: "${env.SERVICE}", text: serviceContent
                        sh """
                            echo "=== VERIFICANDO SERVICIO ${env.SERVICE} ==="
                            SERVICE_EXISTS=\$(ssh -o StrictHostKeyChecking=no ${env.USER}@${env.SERVER} "[ -f /etc/systemd/system/${env.SERVICE} ] && echo yes || echo no")
                            if [ "\$SERVICE_EXISTS" = "no" ]; then
                                echo "Creando unidad de servicio..."
                                scp -o StrictHostKeyChecking=no ${env.SERVICE} ${env.USER}@${env.SERVER}:/tmp/${env.SERVICE}
                                ssh -o StrictHostKeyChecking=no ${env.USER}@${env.SERVER} "
                                    sudo mv /tmp/${env.SERVICE} /etc/systemd/system/${env.SERVICE}
                                    sudo chmod 644 /etc/systemd/system/${env.SERVICE}
                                    sudo systemctl daemon-reload
                                    sudo systemctl enable ${env.SERVICE}
                                "
                                echo "✅ Servicio ${env.SERVICE} creado y habilitado."
                            else
                                echo "El servicio ya existe, omitiendo creación."
                            fi
                        """
                    }
                }
            }
        }

        stage('Deploy') {
            steps {
                sshagent(['server-deploy-key']) {
                    dir("${env.PROJECT_DIR}") {
                        sh """
                            echo "=== DESPLEGANDO ${env.PROJECT_NAME} en ${env.ENVIRONMENT} ==="
                            ssh -o StrictHostKeyChecking=no ${env.USER}@${env.SERVER} "rm -rf ${env.DEPLOY_PATH}/*"
                            ssh -o StrictHostKeyChecking=no ${env.USER}@${env.SERVER} "mkdir -p ${env.DEPLOY_PATH}"
                            scp -o StrictHostKeyChecking=no -r publish/* ${env.USER}@${env.SERVER}:${env.DEPLOY_PATH}/
                            
                            echo "=== CONFIGURANDO SERVICIO ==="
                            ssh -o StrictHostKeyChecking=no ${env.USER}@${env.SERVER} "
                                chown -R ${env.USER}:${env.USER} ${env.DEPLOY_PATH}
                                sudo /usr/bin/systemctl daemon-reload
                                sudo /usr/bin/systemctl restart ${env.SERVICE}
                                echo 'Service status:'
                                sudo /usr/bin/systemctl status ${env.SERVICE} --no-pager | head -5
                            "
                            
                            echo "✅ ${env.PROJECT_NAME} desplegado en ${env.ENVIRONMENT}"
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
            archiveArtifacts artifacts: 'publish/**', allowEmptyArchive: true 
        }
    }
}