using DAL;
using DAL.Repositories;
using DAL.UnitOfWork;
using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class UyeOl : Form
    {
        private Context trenDb;
        private IUnitOfWork uow;
        private IRepository<Kullanici> kullaniciRepo;

        public UyeOl()
        {
            InitializeComponent();
        }

        private void UyeOl_Load(object sender, EventArgs e)
        {
            trenDb = new Context();
            uow = new EFUnitOfWork(trenDb);
            kullaniciRepo = new EFRepository<Kullanici>(trenDb);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chkRezerve tb = new chkRezerve();
            tb.Show();
            this.Hide();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (OrtakMetodlar.BosAlanVarMi(grpUye))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
            else
            {
                Kullanici kullanici = new Kullanici
                {
                    KullaniciTipID = 1,
                   Ad = txtAd.Text,
                   Soyad = txtSoyad.Text,
                  TcNo = txtTcNo.Text,
                   Cinsiyet = rdoErkek.Checked,
                    Adres = txtAdres.Text,
                    Email = txtEposta.Text,
                   Sifre = txtSifre.Text,
                   Telefon = txtTelefon.Text
                };

                //Kullanici kullanici = new Kullanici
                //{
                //    KullaniciTipID = 1,
                //    Ad = "mehmet",
                //    Soyad = "karakaya",
                //    TcNo = "213123343",
                //    Cinsiyet = true,
                //    Adres = "İst",
                //    Email = "mehmet",
                //    Sifre = "123"
                //};

                kullaniciRepo.Add(kullanici);
                int islem = uow.SaveChanges(); 

                   MessageBox.Show("Üyelik işlemi başarıyla gerçekleştirilmiştir.");
                chkRezerve tb = new chkRezerve();
                tb.Show();
                this.Hide();
            }
        }

  


    }
}
