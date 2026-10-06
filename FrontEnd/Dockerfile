# Build stage
FROM node:20 AS build
WORKDIR /app

# Copy package files
COPY FrontEnd/package*.json ./
RUN npm ci

# Install Angular CLI globally to avoid node.exe issue
RUN npm install -g @angular/cli

# Copy source code and build
COPY FrontEnd/ .
RUN ng build --configuration production

# Runtime stage - Nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy built assets from build stage
COPY --from=build /app/dist/ticket-frontend/ .

# Copy nginx configuration
COPY FrontEnd/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]