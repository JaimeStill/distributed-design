#!/bin/bash
dacpac="false"
sqlfiles="false"
SApassword=$1
dacpath=$2
sqlpath=$3

# modify port visibility
gh codespace ports visibility 5001:public -c $CODESPACE_NAME
gh codespace ports visibility 5002:public -c $CODESPACE_NAME

# configure codespace endpoints
replace="s/(?<=https:\/\/)(.+)(?=\-(\d+)\.app\.github\.dev)/$CODESPACE_NAME/g"
proposals="./apps/proposals/src/environments/environment.codespace.ts"
workflows="./apps/workflows/src/environments/environment.codespace.ts"

perl -pe $replace -i $proposals
perl -pe $replace -i $workflows

# install dotnet tools
dotnet tool install -g dotnet-ef

# migrate databases
dotnet ef database update -p ./nodes/proposals/Proposals.Data/
dotnet ef database update -p ./nodes/workflows/Workflows.Data/

# npm update
npm i -g npm

# configure SQL Server
echo "SELECT * FROM SYS.DATABASES" | dd of=testsqlconnection.sql
for i in {1..60};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SApassword -d master -i testsqlconnection.sql > /dev/null
    if [ $? -eq 0 ]
    then
        echo "SQL server ready"
        break
    else
        echo "Not ready yet..."
        sleep 1
    fi
done
rm testsqlconnection.sql

for f in $dacpath/*
do
    if [ $f == $dacpath/*".dacpac" ]
    then
        dacpac="true"
        echo "Found dacpac $f"
    fi
done

for f in $sqlpath/*
do
    if [ $f == $sqlpath/*".sql" ]
    then
        sqlfiles="true"
        echo "Found SQL file $f"
    fi
done

if [ $sqlfiles == "true" ]
then
    for f in $sqlpath/*
    do
        if [ $f == $sqlpath/*".sql" ]
        then
            echo "Executing $f"
            /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SApassword -d master -i $f
        fi
    done
fi

if [ $dacpac == "true" ] 
then
    for f in $dacpath/*
    do
        if [ $f == $dacpath/*".dacpac" ]
        then
            dbname=$(basename $f ".dacpac")
            echo "Deploying dacpac $f"
            /opt/sqlpackage/sqlpackage /Action:Publish /SourceFile:$f /TargetServerName:localhost /TargetDatabaseName:$dbname /TargetUser:sa /TargetPassword:$SApassword
        fi
    done
fi