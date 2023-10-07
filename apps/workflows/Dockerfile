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
COPY ./workflows /usr/local/apps/workflows
WORKDIR /usr/local/apps/workflows
RUN npm i
RUN npm run build

FROM nginx:latest
COPY --from=build /usr/local/apps/workflows/dist/workflows /usr/share/nginx/html
EXPOSE 80