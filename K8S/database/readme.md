# Kubernetes - MSSQL

Run
```
kubectl apply -f mssql-config.yaml
kubectl apply -f mssql-pvc-pv.yaml
kubetcl get pvc
kubectl get pv
kubectl apply -f mssql-deployment.yaml
kubectl apply -f mssql-service.yaml

kubectl get all
kubectl port-forward svc/mssql 1433:1433

```

delete
```


```