FROM gmathieu/node-browsers:3.0.0 AS build

COPY package.json /usr/angular-workdir/
WORKDIR /usr/angular-workdir
RUN npm install

COPY ./ /usr/angular-workdir
RUN npm run build

FROM nginx:1.15.8-alpine

## Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

COPY ./dev/nginx.conf /etc/nginx/nginx.conf

COPY --from=build  /usr/angular-workdir/dist/signal-monitoring-ui /usr/share/nginx/html
EXPOSE 4200
EXPOSE 49153
RUN echo "for mainFileName in /usr/share/nginx/html/main*.js ;\
            do \
              envsubst '\$BACKEND_API_URL \$DEFAULT_LANGUAGE ' < \$mainFileName > main.tmp ;\
              mv main.tmp \${mainFileName} ;\
            done \
            && nginx -g 'daemon off;'" > run.sh

ENTRYPOINT ["sh", "run.sh"]
