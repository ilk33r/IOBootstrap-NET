#!/usr/bin/env bash

USAGE="USAGE:
    io.service.staging.sh [start|stop|restart|status]
"
SERVICE_NAME="IO"
BATCH_SERVICE_NAME="IO.Batch"
SERVICE_FOLDER="/tmp/io-service"
PID_FILE="${SERVICE_FOLDER}/io.service.pid"
BATCH_PID_FILE="${SERVICE_FOLDER}/io.batch.pid"
LOG_FILE="${SERVICE_FOLDER}/io.service.log"
BATCH_LOG_FILE="${SERVICE_FOLDER}/io.batch.log"
PORT_NUMBER="7000"
ENVIRONMENT="Staging"
EXEC="dotnet exec ${PWD}/IOBootstrap.NET.Default.Application.dll"
BATCH_EXEC="dotnet exec ${PWD}/IOBootstrap.NET.Default.Batch.dll ${ENVIRONMENT}"

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

batchPidValue() {
    batch_ret_val=""

    if [ -f "${BATCH_PID_FILE}" ];
    then
        batch_ret_val="$(cat ${BATCH_PID_FILE})"
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

    local batch_ret_val=""
    batchPidValue
    
    if [ "${batch_ret_val}" == "" ];
    then
        echo "Service ${BATCH_SERVICE_NAME} is not running"
    else
        echo "Serive ${BATCH_SERVICE_NAME} is runnig with PID ${batch_ret_val}"
    fi
}

checkPort() {
    lsof -i:${PORT_NUMBER}
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
    export "ASPNETCORE_URLS=https://+:${PORT_NUMBER}"
    export "SERVER_PORT=${PORT_NUMBER}"
    export "ASPNETCORE_Kestrel__Certificates__Default__Password=io"

    nohup -- bash -c "${EXEC}" 0<&- &> "${LOG_FILE}" & pid=$!
    echo $pid > $PID_FILE

    nohup -- bash -c "${BATCH_EXEC}" 0<&- &> "${BATCH_LOG_FILE}" & batchPid=$!
    echo $batchPid > $BATCH_PID_FILE

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

    local batch_ret_val=""
    batchPidValue
    if [ "${batch_ret_val}" == "" ];
    then
        echo "Service ${BATCH_SERVICE_NAME} is not running"
        exit 0
    fi

    echo "Stopping ${BATCH_SERVICE_NAME}..."
    kill -s TERM $batch_ret_val
    wait
    rm "$BATCH_PID_FILE"
    echo "OK"
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
    "check-port")
        checkPort
        ;;
    *)
        showUsage
        ;;
esac
done
