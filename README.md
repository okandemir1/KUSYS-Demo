# KUSYS-Demo

1.Adım
Db Ayarları
KUSYSDEMO adında bir db oluşturunuz.
connectionstring'i aşağıdaki gibi kullanabilirsiniz
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=KUSYSDEMO;Password=123456;User Id=sa;MultipleActiveResultSets=True;TrustServerCertificate=True;Application Name=KUSYSDEMO"
},
2.Adım
Db Tablo Eklenmesi
1-KUSYS.Application.WebUI > Set as Startup Project durumunda iken
2-Tools > NuGet Package Manager > Package Manager Console'u açınız
3-Console açıldıktan sonra Default Project kısmını "KUSYS.Data" olarak seçin
4-Console içine "Add-Migration -Context KUSYSDbContext" yazınız
5-İsim veriniz örn:a1
6-Migration oluşturma başarılı olduktan sonra Console içine "Update-Database -Context KUSYSDbContext" yazınız

Db içerisinde tablolar eklendi ise artık demo verileri yükleyebilirsiniz
Giriş ekranında bulunan demo verileri yükle butonuna tıklayabilirsiniz
kullanıcı adı : o şifre : 1 olarak giriş yapabilirsiniz
