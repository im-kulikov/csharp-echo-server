# Fast and scalable implementation of webserver on Dotnet Core + KestelHttpServer

This is Http Echo server example

## Benchmarks

`wrk -c 200 -t 8 -d 10 http://172.34.0.2:8080`

```
Requests/sec: 219446.23 - (24 Cores, 16Gb RAM)
Requests/sec:  49814.28 - (8 Cores, 16Gb RAM)
```