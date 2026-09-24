docker build \
  -f src/CSharpApp.Api/Dockerfile \
  -t csharpapp-api .

docker run --rm \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  --name csharpapp-api \
  csharpapp-api