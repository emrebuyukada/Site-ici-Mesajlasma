using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace WebApplication7
{
    public class PostHub : Hub<IPostHubClient>
    {
        static int count = 0;
        public static ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();
        static string ip1 = "";

        public override Task OnConnected()
        {
            count++;
       
            return base.OnConnected();
       }
       
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
          
            string yol = System.Web.Hosting.HostingEnvironment.MapPath("/") + "veritabani.accdb";
            OleDbConnection db_baglanti;
            db_baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; DATA Source=" + yol + ""); try
            {
                db_baglanti.Open();
                OleDbCommand db_komut = new OleDbCommand("Update  kullanici set  durum='ofline' where  id='"+ Context.ConnectionId+"'; ", db_baglanti);
                db_komut.ExecuteNonQuery();
                db_baglanti.Close();

            }
            catch
            {
                //Response.Write("Bağlantıda sorun var !!!");
            }

           

            count = count - 1;
            var userName = string.Empty;
            ConnectedUsers.TryRemove(Context.ConnectionId, out userName);
            return base.OnDisconnected(stopCalled);
        }
        public void ReceiveMessage(string mesaj,string to)
        {
          
            var connectionId = ConnectedUsers.FirstOrDefault(u => u.Value == to);

          //  Clients.Client(connectionId.Key).Test("alert("+mesaj+");");

            if (connectionId.Key!=null)
            {
                
                Clients.Client(connectionId.Key).Test(mesaj);
            }
           
        }
    
        static int sayi=0;//kayit yapilan kisiye özgü
        public void ipkayit(string ip)
        {
            ip1 = ip;
        }
        public void yukle()
        {
            string yol = System.Web.Hosting.HostingEnvironment.MapPath("/") + "veritabani.accdb";
            OleDbConnection bg = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; DATA Source=" + yol + "");
            bg.Open(); //Burda Sorgumuzu yazıp çalıştırıyoruz
            string sorgu = "select nick,durum,ip from kullanici";
            OleDbCommand komut = new OleDbCommand(sorgu, bg);
            OleDbDataAdapter adap = new OleDbDataAdapter(komut);
            DataTable data = new DataTable();
            adap.Fill(data);
            bg.Close();

            string a = "";
            string b = "";
            string tamami = "";
            for (int i = 0; i < data.Rows.Count; i++)
            {
                a = data.Rows[i][0].ToString();
                b = data.Rows[i][1].ToString();
                tamami += a + "[" + b + "]" + "<br>";

            }
            Clients.Caller.Test1(tamami);
        }
        public void cikis()
        {
            string yol = System.Web.Hosting.HostingEnvironment.MapPath("/") + "veritabani.accdb";
            OleDbConnection db_baglanti;
            db_baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; DATA Source=" + yol + ""); try
            {
                db_baglanti.Open();
                OleDbCommand db_komut = new OleDbCommand("Update  kullanici set  durum='ofline' where ip='" + ip1 + "'; ", db_baglanti);
                db_komut.ExecuteNonQuery();
                db_baglanti.Close();

            }
            catch
            {
                //Response.Write("Bağlantıda sorun var !!!");
            }
          
           

        }
        public void Join(string username)
        {
        
            ConnectedUsers.AddOrUpdate(Context.ConnectionId, username, (k,v) => username);
           // Clients.Caller.Test(username+"  Hosgeldin..");

            string yol = System.Web.Hosting.HostingEnvironment.MapPath("/")+ "veritabani.accdb";
            OleDbConnection db_baglanti;
            db_baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; DATA Source="+yol+"" );
            try
            {
                db_baglanti.Open();
                OleDbCommand db_komut = new OleDbCommand("Insert INTO kullanici ( nick, ip ,durum,id) values('"+username+"','"+ ip1 + "','online','"+ Context.ConnectionId+"') ", db_baglanti);
                db_komut.ExecuteNonQuery();
                db_baglanti.Close();

            }
            catch
            {
               // Response.Write("Bağlantıda sorun var !!!");
            }

            OleDbConnection bg = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; DATA Source=" + yol + "");
            bg.Open(); //Burda Sorgumuzu yazıp çalıştırıyoruz
            string sorgu = "select nick,durum,ip from kullanici";
            OleDbCommand komut = new OleDbCommand(sorgu, bg);
            OleDbDataAdapter adap = new OleDbDataAdapter(komut);
            DataTable data = new DataTable();
            adap.Fill(data);
            bg.Close();

            string a = "";
            string b = "";
            string tamami = "";
            for (int i = 0; i < data.Rows.Count; i++)
            {
                a = data.Rows[i][0].ToString();
                b = data.Rows[i][1].ToString();
                tamami += a + "[" + b + "]" + "<br>";

            }
            Clients.Caller.Test1(tamami);
            sayi++;
        }

    }
    public interface IPostHubClient
    {

        void Test(string meesage);
        void Test1(string kisi);

    }
}