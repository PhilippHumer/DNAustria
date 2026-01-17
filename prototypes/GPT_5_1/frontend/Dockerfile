# Frontend Angular Dockerfile (Dev server)
FROM node:20-alpine
WORKDIR /app
COPY ./frontend/package*.json ./
RUN npm install
COPY ./frontend/ ./
EXPOSE 8080
CMD ["npm","start","--","--host","0.0.0.0","--port","8080"]

