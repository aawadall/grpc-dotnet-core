:: Original material obtained from https://stackoverflow.com/questions/37714558/how-to-enable-server-side-ssl-for-grpc/37739265#37739265

@echo off
set OPENSSL_CONF=C:\Program Files\OpenSSL-Win64\bin\openssl.cfg

echo Generate CA key:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" genrsa -passout pass:1111 -des3 -out ca.key 4096

echo Generate CA certificate:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" req -passin pass:1111 -new -x509 -days 365 -key ca.key -out ca.crt -subj  "/C=US/ST=CA/L=Cupertino/O=YourCompany/OU=YourApp/CN=MyRootCA"

echo Generate server key:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" genrsa -passout pass:1111 -des3 -out server.key 4096

echo Generate server signing request:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" req -passin pass:1111 -new -key server.key -out server.csr -subj  "/C=US/ST=CA/L=Cupertino/O=YourCompany/OU=YourApp/CN=%COMPUTERNAME%"

echo Self-sign server certificate:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" x509 -req -passin pass:1111 -days 365 -in server.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out server.crt

echo Remove passphrase from server key:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" rsa -passin pass:1111 -in server.key -out server.key

echo Generate client key
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" genrsa -passout pass:1111 -des3 -out client.key 4096

echo Generate client signing request:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" req -passin pass:1111 -new -key client.key -out client.csr -subj  "/C=US/ST=CA/L=Cupertino/O=YourCompany/OU=YourApp/CN=%CLIENT-COMPUTERNAME%"

echo Self-sign client certificate:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" x509 -passin pass:1111 -req -days 365 -in client.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out client.crt

echo Remove passphrase from client key:
"C:\Program Files\OpenSSL-Win64\bin\openssl.exe" rsa -passin pass:1111 -in client.key -out client.key