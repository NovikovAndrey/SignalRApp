FROM node:latest
WORKDIR /app
COPY package.json .
RUN npm install
COPY . .
EXPOSE 4200
EXPOSE 49153
CMD ["npm", "start"]