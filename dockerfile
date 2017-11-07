FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out
FROM microsoft/dotnet:runtime
RUN apt-get update && apt-get install unzip
WORKDIR /app
COPY run.sh .
COPY --from=build-env /app/out ./
RUN ["chmod", "+x", "run.sh"]
CMD ["./run.sh"]
EXPOSE 80
