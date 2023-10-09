FROM node:lts-alpine as build
COPY ./libs /usr/local/apps/libs
WORKDIR /usr/local/apps/libs/core
RUN npm i
RUN npm run build
WORKDIR /usr/local/apps/libs/contracts/core
RUN npm i
RUN npm run build
WORKDIR /usr/local/apps/libs/contracts/workflows
RUN npm i
RUN npm run build
WORKDIR /usr/local/apps/libs/distributed
RUN npm i
RUN npm run build
COPY ./proposals /usr/local/apps/proposals
WORKDIR /usr/local/apps/proposals
RUN npm i
RUN npm run build

FROM nginx:latest
COPY --from=build /usr/local/apps/proposals/dist/proposals /usr/share/nginx/html
EXPOSE 80