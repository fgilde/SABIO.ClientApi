# SABIO.ClientApi
An api client for SABIO aka Serviceware Knowledge from https://www.getsabio.com/de/

```
 var client = new SabioClient("https://<your sabio url>/sabio/services", "helpline");
 var r = client.Apis.Authentication.LoginAsync("test", "abc").Result;
 var texts = client.Apis.Texts.GetAllAsync().Result;
```