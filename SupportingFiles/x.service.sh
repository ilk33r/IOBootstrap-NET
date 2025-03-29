#!/usr/bin/env bash

USAGE="USAGE:
    appcell.service.sh [start|stop|restart|status]
"
SERVICE_NAME="AppCell"
SERVICE_FOLDER="/tmp/appcell-service"
PID_FILE="${SERVICE_FOLDER}/appcell.service.pid"
LOG_FILE="${SERVICE_FOLDER}/appcell.service.log"
PORT_NUMBER="7000"
ENVIRONMENT="Release"
EXEC="dotnet exec ${PWD}/MobileAppBetaDistributions.Application.dll"

showUsage() {
	echo -n "$USAGE"
	exit 0
}

checkServiceFolder() {
    if [ ! -d "${SERVICE_FOLDER}" ];
    then
	    mkdir -p "${SERVICE_FOLDER}"
    fi
}

pidValue() {
    ret_val=""

    if [ -f "${PID_FILE}" ];
    then
        ret_val="$(cat ${PID_FILE})"
    fi
}

status() {
    checkServiceFolder

    local ret_val=""
    pidValue
    
    if [ "${ret_val}" == "" ];
    then
        echo "Service ${SERVICE_NAME} is not running"
    else
        echo "Serive ${SERVICE_NAME} is runnig with PID ${ret_val}"
    fi
}

start() {
    checkServiceFolder

    local ret_val=""
    pidValue
    if [ "${ret_val}" != "" ];
    then
        echo "Serive ${SERVICE_NAME} is runnig with PID ${ret_val}"
        exit 0
    fi

    echo "Starting ${SERVICE_NAME}..."
    export "ASPNETCORE_ENVIRONMENT=${ENVIRONMENT}"
    export "ASPNETCORE_URLS=http://+:${PORT_NUMBER}"
    export "SERVER_PORT=${PORT_NUMBER}"

    nohup -- bash -c "${EXEC}" 0<&- &> "${LOG_FILE}" & pid=$!
    echo $pid > $PID_FILE
    status
}

stop() {
    checkServiceFolder

    local ret_val=""
    pidValue
    if [ "${ret_val}" == "" ];
    then
        echo "Service ${SERVICE_NAME} is not running"
        exit 0
    fi

    echo "Stopping ${SERVICE_NAME}..."
    kill -s TERM $ret_val
    wait
    rm "$PID_FILE"
    status
}

if [ "$#" -ne 1 ]; then
	showUsage
fi

for i in "$@"
do
case $i in
    "start")
        start
        ;;
    "stop")
        stop
        ;;
    "restart")
        stop
        start
        ;;
    "status")
        status
        ;;
    *)
        showUsage
        ;;
esac
done
