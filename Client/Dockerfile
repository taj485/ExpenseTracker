# Build context is this directory (Client/), not the repo root — the root
# .dockerignore excludes Client, and Docker reads .dockerignore from the
# context root, so Client/.dockerignore is what applies here.
#
#   docker build -t expensetracker-web ./Client

# Node 22 to match the "engines" constraint in package.json.
FROM node:22-alpine AS build
WORKDIR /src

# Install from the lockfile first, so editing source does not re-install.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .
# "selfhost" points apiUrl at /api so the app calls the API through nginx on
# the same origin. The default "production" config would bake in the Azure URL.
# The application builder emits browser assets to dist/<project>/browser.
RUN npm run build -- --configuration selfhost

FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /src/dist/expense-tracker/browser /usr/share/nginx/html
EXPOSE 80
