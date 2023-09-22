echo "Configuring Codespaces Endpoints"

replace="s/(?<=https:\/\/)(.+)(?=\-(\d+)\.app\.github\.dev)/$CODESPACE_NAME/g"
proposals="./apps/proposals/src/environments/environment.codespace.ts"
workspaces="./apps/workspaces/src/environments/environment.codespace.ts"

perl -pe $replace -i $proposals
perl -pe $replace -i $workspaces
