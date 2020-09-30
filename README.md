# HyperV-NAT-Port-Forwarding-UI
create a .NET tool to admin the NAT forwading rules in HyperV host

## the function was not working properly.


## Windows CMD

## 1-Query
```netsh interface portproxy show v4tov4```
## 2-Query By IP
```netsh interface portproxy show v4tov4|find "0.0.0.0" ```
## 3-Add
```netsh interface portproxy add v4tov4 listenport=HOSTPORT listenaddress=HOSTIP connectaddress=VMIP connectport=VMPORT```
## 4-Add-Demo
```netsh interface portproxy add v4tov4 listenport=8888 listenaddress=0.0.0.0 connectaddress=192.168.137.2 connectport=3389```
## 5-Delete
```netsh interface portproxy delete v4tov4 listenaddress=HOSIP listenport=HOSTPORT```
## 6-Delete-Demo
```netsh interface portproxy delete v4tov4  listenaddress=0.0.0.0 listenport=8888```
