using DAL;
using DAL.Repositories;
using DAL.UnitOfWork;
using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class TrenBilet : Form
    {
        #region DbRepo
        private Context trenDb;
        private IUnitOfWork uow;
        private IRepository<Kullanici> kullaniciRepo;
        private IRepository<Bilet> biletRepo;
        private IRepository<Durak> durakRepo;
        private IRepository<Rota> rotaRepo;
        private IRepository<Sefer> seferRepo;
        private IRepository<Tren> trenRepo;
        private IRepository<Vagon> vagonRepo;
        private IRepository<TrenVagon> trenVagonRepo;
        private IRepository<VagonTipi> vagonTipiRepo;
        Bilet bilet;
        Sefer sefer;
        bool cinsiyet;
        #endregion
        int donusBileti = 0;        //Gidiş dönüş bileti isteniyorsa bu değer 1 olur, gidiş  bileti alınınca 2 olur.
        int gidisSeferID, donusSeferID, cikisId, varisId;
        DateTime gidisTarihi, donusTarihi;
        int biletSinifi; //1 ekonomi 2 business
        int yolcuSayisi;
        int secilenKoltuk;
        string sinif;
        Kullanici kullanici;
        int fiyat = 70;
        Cinsiyet cns;

        public void DbInitialize()
        {
            trenDb = new Context();
            // EFBlogContext'i kullanıyor olduğumuz için EFUnitOfWork'den türeterek constructor'ına
            // ilgili context'i constructor injection yöntemi ile inject ediyoruz.
            uow = new EFUnitOfWork(trenDb);
            kullaniciRepo = new EFRepository<Kullanici>(trenDb);
            biletRepo = new EFRepository<Bilet>(trenDb);
            durakRepo = new EFRepository<Durak>(trenDb);
            rotaRepo = new EFRepository<Rota>(trenDb);
            seferRepo = new EFRepository<Sefer>(trenDb);
            trenRepo = new EFRepository<Tren>(trenDb);
            vagonRepo = new EFRepository<Vagon>(trenDb);
            trenVagonRepo = new EFRepository<TrenVagon>(trenDb);
            vagonTipiRepo = new EFRepository<VagonTipi>(trenDb);
        }
        public TrenBilet()
        {
            InitializeComponent();
            DbInitialize();
            bilet = new Bilet();
            sefer = new Sefer();
            #region DuraklariDoldur
            var duraklar = durakRepo.GetAll().ToList();

            foreach (var item in duraklar)
            {
                ComboboxItem itemCmb = new ComboboxItem();
                itemCmb.Text = item.DurakAdi;
                itemCmb.Value = item.DurakID;

                cmbNereden.Items.Add(itemCmb);
            }
            #endregion

            grpKoltukBusiness.Visible = false;
            grpKoltukEkonomi.Visible = false;
        }


        private void TrenBilet_Load(object sender, EventArgs e)
        {
            //Tableri gizle
            /*  TrenTab.Appearance = TabAppearance.FlatButtons;
              TrenTab.ItemSize = new Size(0, 1);
              TrenTab.SizeMode = TabSizeMode.Fixed;*/

            dtGidis.MinDate = DateTime.Today;
            dtGidis.MaxDate = DateTime.Today.AddDays(14);
            dtDonus.MinDate = DateTime.Today;
            dtDonus.MaxDate = DateTime.Today.AddDays(14);

        }
        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            var girisYapan = kullaniciRepo.Get(x => x.Email == txtEposta.Text && x.Sifre == txtSifre.Text);

            if (girisYapan != null)
            {
                kullanici = new Kullanici();
                kullanici.KullaniciID = girisYapan.KullaniciID;
                TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
            }
            else
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
        }

        private void btnUyeOl_Click(object sender, EventArgs e)
        {
            UyeOl uyelik = new UyeOl();
            uyelik.Show();
            this.Hide();
        }

        private void btnDevamEt_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
        }

        private void BiletDurak_Click(object sender, EventArgs e)
        {

        }

        private void cmbNereden_SelectedIndexChanged(object sender, EventArgs e)
        {
            var duraklar = durakRepo.GetAll().ToList();
            cmbNereye.Items.Clear();

            foreach (var item in duraklar)
            {
                ComboboxItem itemCmb = new ComboboxItem();
                itemCmb.Text = item.DurakAdi;
                itemCmb.Value = item.DurakID;

                if (cmbNereden.Text != itemCmb.Text)
                {
                    cmbNereye.Items.Add(itemCmb);
                }
            }
        }

        private void rdoGidisDonus_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoGidisDonus.Checked)
            {
                dtDonus.Enabled = true;
            }
        }

        private void rdoTekyon_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTekyon.Checked)
            {
                dtDonus.Enabled = false;
                donusBileti = 0;
            }
            else
            {
                dtDonus.Enabled = true;
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            yolcuSayisi = Convert.ToInt32(nmrYolcuSayisi.Value);
            donusBileti = 0;
            if (rdoGidisDonus.Checked)
            {
                donusBileti = 1;

            }
            if (OrtakMetodlar.BosAlanVarMi(grpDurak))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
            else
            {
                lblSeferBilgi.Text = "Lütfen gidiş seferini seçiniz.";
                cikisId = Convert.ToInt32(((ComboboxItem)cmbNereden.SelectedItem).Value);
                varisId = Convert.ToInt32(((ComboboxItem)cmbNereye.SelectedItem).Value);
                gidisTarihi = new DateTime(dtGidis.Value.Year, dtGidis.Value.Month, dtGidis.Value.Day, 0, 0, 0);

                SeferleriGetir(cikisId, varisId, gidisTarihi);
                TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;

            }
        }

        private void pbBusiness_Click(object sender, EventArgs e)
        {
            grpKoltukBusiness.Visible = true;
            grpKoltukEkonomi.Visible = false;

            biletSinifi = 2;
        }

        private void pbEko_Click(object sender, EventArgs e)
        {
            grpKoltukBusiness.Visible = false;
            grpKoltukEkonomi.Visible = true;

            biletSinifi = 1;

        }

        private void koltukSecildi(object sender, EventArgs e)
        {
            PictureBox secilenPb = (sender as PictureBox);
            cns = new Cinsiyet();
            cns.Show();
            cinsiyet = cns.rdoErkek.Checked;
            secilenKoltuk = Convert.ToInt32((sender as PictureBox).Tag.ToString());

            /*   foreach (Control item in grpKoltukBusiness.Controls)
               {
                   if (item is PictureBox)
                   {
                       if (((PictureBox)item).Image == UI.Properties.Resources.secili)
                           ((PictureBox)item).Image = UI.Properties.Resources.bos1; }
               }
               foreach (Control item in grpKoltukEkonomi.Controls)
               {
                   if (item is PictureBox)
                   {
                       if (((PictureBox)item).Image == UI.Properties.Resources.secili)
                           ((PictureBox)item).Image = UI.Properties.Resources.bos1;
                   }
               }*/

            KoltuklariDoldur();

            secilenPb.Image = UI.Properties.Resources.secili;


            sinif = biletSinifi == 2 ? "Business" : "Ekonomi";
            MessageBox.Show(sinif + " " + secilenKoltuk.ToString());
        }

        private void KoltuklariDoldur()
        {
            //İlgili sefere ait satılmış veya rezerve edilmiş biletleri koltuklara doldur.

            if (biletSinifi == 1)  //Ekonomi
            {
                var alinmisBiletler = biletRepo.Get(x => x.BiletID == gidisSeferID);

                if (alinmisBiletler.BiletDurumu == true && alinmisBiletler.Cinsiyet == true)  //satın alınan biletler
                {
                    string pbName = "e" + alinmisBiletler.KoltukNo;
                    PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox ;
                    pb.Image = UI.Properties.Resources.kadinSatinAl;

                }
                else    //Business
                {     

                }
            }
        }

        private void BiletKoltuk_Click(object sender, EventArgs e)
        {

        }

        private void FiyatBelirle()
        {
            if (rdoCocuk.Checked)
            {
                fiyat -= 35;
            }
            if (chkSigortali.Checked)
            {
                fiyat += 10;
            }
            if (chkYemekli.Checked)
            {
                if (biletSinifi == 1)
                    fiyat += 20;
                else
                {
                    fiyat += 15;
                }
            }
            if (ckhEkstraHizmet.Checked)
            {
                fiyat += 10;
            }
        }

        private void btnSatinAl_Click(object sender, EventArgs e)
        {
            bool biletDurum = ((Button)sender).Name == "btnSatinAl" ? true : false;
            FiyatBelirle();

            Bilet bilet = new Bilet
            {
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                TcNo = txtTc.Text,
                BiletDurumu = biletDurum,     //Satın almada true, rezervasyonda false
                Cinsiyet = cinsiyet,
                CocukMu = rdoCocuk.Checked,
                SigortaliMi = chkSigortali.Checked,
                YemekliMi = chkYemekli.Checked,
                KoltukNo = secilenKoltuk,
                YolculukHizmetiVarMi = ckhEkstraHizmet.Checked,
                SeferID = gidisSeferID,
                KullaniciID = kullanici.KullaniciID,
                Fiyat = fiyat,
                VagonSinifi = biletSinifi == 1 ? false : true   //biletSinifi = 1 ise ekonomi seçilmiştir. vagon sınıfı false olur. 1 değilse business seçilmiştir. Vagon sınıfı true olur.
            };

            biletRepo.Add(bilet);
            uow.SaveChanges();
        }




        public void SeferleriGetir(int cikisId, int varisId, DateTime gidisTarihi)
        {
            var rotaId = rotaRepo.GetAll(x => x.CikisID == cikisId && x.VarisID == varisId).Select(x => x.RotaID).SingleOrDefault();
            var seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi).ToList();
            var deneme = seferRepo.GetAll().Select(x => DbFunctions.TruncateTime(x.Tarih)).ToList();
            lstSeferler.Items.Clear();

            foreach (var item in seferler)
            {
                ListViewItem item1 = new ListViewItem(item.SeferID.ToString());
                item1.SubItems.Add(cmbNereden.Text);
                item1.SubItems.Add(cmbNereye.Text);
                item1.SubItems.Add(item.CikisSaati.ToString());
                item1.SubItems.Add(item.VarisSaati.ToString());
                item1.SubItems.Add(item.Tarih.ToString());
                item1.SubItems.Add(item.SeferSuresi.ToString());
                lstSeferler.Items.Add(item1);

            }

        }

        private void btnIleriSefer_Click(object sender, EventArgs e)
        {
            if (donusBileti == 1)
            {
                lblSeferBilgi.Text = "Lütfen dönüş seferini seçiniz.";
                //Dönüş bileti isteniyorsa tekrar bu sayfaya dön. dönüş seferlerini listele.
                varisId = Convert.ToInt32(((ComboboxItem)cmbNereden.SelectedItem).Value);
                cikisId = Convert.ToInt32(((ComboboxItem)cmbNereye.SelectedItem).Value);
                donusTarihi = new DateTime(dtDonus.Value.Year, dtGidis.Value.Month, dtGidis.Value.Day, 0, 0, 0);
                SeferleriGetir(cikisId, varisId, donusTarihi);

                donusBileti = 2;
            }
            else
            {
                TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
            }
        }

        private void lstSeferler_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (donusBileti == 1 || donusBileti == 0)
            {
                gidisSeferID = Convert.ToInt32(lstSeferler.SelectedItems[0].SubItems[0].Text);
                MessageBox.Show("Gidiş seferi id: " + gidisSeferID);
            }
            else
            {
                donusSeferID = Convert.ToInt32(lstSeferler.SelectedItems[0].SubItems[0].Text);
                MessageBox.Show("Dönüş seferi id: " + donusSeferID);
            }
        }

        private void BiletSefer_Click(object sender, EventArgs e)
        {
        }
    }



}


