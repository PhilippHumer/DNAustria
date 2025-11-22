# Build
FROM node:20 AS build
WORKDIR /app
COPY . .
RUN npm install && npm run build -- --configuration production

# Serve
FROM nginx:alpine as runtime
COPY --from=build /app/dist/frontend /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

