pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-u root:root -v /var/run/docker.sock:/var/run/docker.sock'
        }
    }

    // parameters {
    //     choice(
    //         name: 'PROJECT',
    //         choices: ['web', 'worker'],
    //         description: 'Proyecto a desplegar'
    //     )
    //     choice(
    //         name: 'ENVIRONMENT',
    //         choices: ['pre', 'pro'],
    //         description: 'Ambiente de despliegue'
    //     )
    //     booleanParam(
    //         name: 'SKIP_BUILD',
    //         defaultValue: false,
    //         description: 'Saltar build (solo redeploy)'
    //     )
    // }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
        DOTNET_CLI_HOME = '/tmp/dotnet-home'

        // Variables según parámetros
        USER = 'mrodriguex'
        SERVER = '62.72.3.22'
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
                    env.PROJECT_DIR = params.PROJECT == 'web' ? 'Analitica' : 'AnaliticaWorker'
                    env.PROJECT_NAME = params.PROJECT == 'web' ? 'Analitica' : 'AnaliticaWorker'
                    env.DEPLOY_DIR = params.PROJECT == 'web' ? 'analitica' : 'analiticaWorker'
                    env.DEPLOY_PATH = "/home/${env.USER}/www/analitica.net/${params.ENVIRONMENT}/${env.DEPLOY_DIR}"
                    env.SERVICE = params.PROJECT == 'web' 
                        ? "analitica-${params.ENVIRONMENT}.service"
                        : "analitica-worker-${params.ENVIRONMENT}.service"
                    
                    echo "Configuración:"
                    echo "  PROYECTO: ${params.PROJECT}"
                    echo "  AMBIENTE: ${params.ENVIRONMENT}"
                    echo "  SERVICIO: ${env.SERVICE}"
                    echo "  PATH: ${env.DEPLOY_PATH}"
                }
            }
        }

        stage('Build') {
            steps {
                dir("${PROJECT_DIR}") {
                    sh "dotnet restore ${PROJECT_NAME}.csproj"
                    sh "dotnet build ${PROJECT_NAME}.csproj -c Release"
                    sh "dotnet publish ${PROJECT_NAME}.csproj -c Release -o ./publish"
                    sh '''
                        echo "=== BUSCANDO appsettings ==="
                        find . -name "appsettings.json"
                    '''
                    sh "cat ./publish/appsettings.json"
                    echo "✅ Build completado para ${params.PROJECT} en ${params.ENVIRONMENT}"
                }
            }
        }

        stage('Deploy') {
            steps {
                sshagent(['server-deploy-key']) {
                    dir("${PROJECT_DIR}") {
                        sh """
                            echo "=== DESPLEGANDO ${params.PROJECT} en ${params.ENVIRONMENT} ==="
                            ssh -o StrictHostKeyChecking=no ${USER}@${SERVER} "rm -rf ${DEPLOY_PATH}/*"
                            ssh -o StrictHostKeyChecking=no ${USER}@${SERVER} "mkdir -p ${DEPLOY_PATH}"
                            scp -o StrictHostKeyChecking=no -r publish/* ${USER}@${SERVER}:${DEPLOY_PATH}/
                            
                            echo "=== CONFIGURANDO SERVICIO ==="
                            ssh -o StrictHostKeyChecking=no ${USER}@${SERVER} "
                                chown -R ${USER}:${USER} ${DEPLOY_PATH}
                                sudo /usr/bin/systemctl daemon-reload
                                sudo /usr/bin/systemctl restart ${SERVICE}
                                echo 'Service status:'
                                sudo /usr/bin/systemctl status ${SERVICE} --no-pager | head -5
                            "
                            
                            echo "✅ ${params.PROJECT} desplegado en ${params.ENVIRONMENT}"
                        """
                    }
                }
            }
        }
    }

    post {
        success { 
            echo "✅ PIPELINE COMPLETADO - ${params.PROJECT} en ${params.ENVIRONMENT}"
        }
        failure { 
            echo "❌ FALLÓ - ${params.PROJECT} en ${params.ENVIRONMENT}"
        }
        always { 
            archiveArtifacts artifacts: 'publish/**', allowEmptyArchive: true 
        }
    }
}