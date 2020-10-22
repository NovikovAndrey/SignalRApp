FROM node:latest
WORKDIR /app
COPY package.json .
COPY package-lock.json .
RUN npm install
COPY . .
RUN npm install -g @angular/cli
RUN ng build --configuration production --output-path=/dist
EXPOSE 4200
EXPOSE 49153
CMD npm run start