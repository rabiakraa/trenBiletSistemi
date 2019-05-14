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
    public partial class chkRezerve : Form
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

        #region Diğer Değişkenler
        int donusBileti = 0;        //Gidiş dönüş bileti isteniyorsa bu değer 1 olur, gidiş  bileti alınınca 2 olur.
        int gidisSeferID, donusSeferID, cikisId, varisId;
        DateTime gidisTarihi, donusTarihi;
        int biletSinifi; //1 ekonomi 2 business
        int yolcuSayisi;
        int aktifYolcu = 1;
        int secilenKoltuk;
        string sinif;
        Kullanici kullanici;
        int fiyat = 70;
        Cinsiyet cns;
        int koltukGidis = 0;   //Dönüş bileti de alınacaksa 1 olur.
        bool biletDurum;        //satın almaysa true rezervasyonsa false
        PictureBox secilenPb;
        bool ayniKullanici = false;//alınan biletleri aynı kullanıcı alıyorsa kız erkek ayrımı yapma.
        bool uyeDegil = false;      //Uye olmadan giriş yapılması durumunu kontrol eder.
        #endregion

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

        public chkRezerve()
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
            //Tab butonlarını gizle
            /*TrenTab.Appearance = TabAppearance.FlatButtons;
             TrenTab.ItemSize = new Size(0, 1);
             TrenTab.SizeMode = TabSizeMode.Fixed;*/

            //Geçmişe dair bilet alınmasını engellemek ve en fazla 14 gün sonrasına bilet vermek için.
            dtGidis.MinDate = DateTime.Today;
            dtGidis.MaxDate = DateTime.Today.AddDays(14);
            dtDonus.MinDate = DateTime.Today;
            dtDonus.MaxDate = DateTime.Today.AddDays(14);

        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            uyeDegil = false;
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
            uyeDegil = true;
            kullanici = new Kullanici();
            kullanici.KullaniciID = 1;      //Üye olmadan devam edildiği takdirde kullanıcı id olarak default bir user'ın kullanıcıID'si verilir.
            TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
        }

        private void cmbNereden_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Nereden comboboxından seçilen durağa göre nereye comboxını doldur. Örneğin İstanbul seçilirse varış durağı olarak İstanbul harici tüm duraklar alınır.
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
                //Aşağıdaki iki değişken dönüş seferi seçimi ve dönüş biletlerinin alınması için gereklidir.
                donusBileti = 1;
                koltukGidis = 1;
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
            biletDurum = rdoSatinAl.Checked;
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

                if (donusBileti == 1)    //Gidiş dönüş seçilmişse
                {
                    if (SeferVarMi(cikisId, varisId, gidisTarihi) && SeferVarMi(varisId, cikisId, donusTarihi))
                    {
                        TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
                    }
                    else
                    {
                        MessageBox.Show("Sefer(ler) bulunamadı.");
                    }
                }

                else if (donusBileti == 0)    //Tek yön seçilmişse
                {
                    if (SeferVarMi(cikisId, varisId, gidisTarihi))
                    {
                        TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
                    }
                    else
                    {
                        MessageBox.Show("Sefer bulunamadı.");
                    }
                }

                SeferleriGetir(cikisId, varisId, gidisTarihi);

            }

        }

        private void pbBusiness_Click(object sender, EventArgs e)
        {
            grpKoltukBusiness.Visible = true;
            grpKoltukEkonomi.Visible = false;

            biletSinifi = 2;
            KoltuklariDoldur(gidisSeferID);

            lblFiyat.Text = "100";
            fiyat = 100;

        }

        private void pbEko_Click(object sender, EventArgs e)
        {
            grpKoltukBusiness.Visible = false;
            grpKoltukEkonomi.Visible = true;

            biletSinifi = 1;
            KoltuklariDoldur(gidisSeferID);
            lblFiyat.Text = "70";
            fiyat = 70;

        }

        private void koltukSecildi(object sender, EventArgs e)
        {
            //Tıklanan koltuk secilenPb isimli PictureBox'ta tutulur.
            secilenPb = (sender as PictureBox);
            //Seçilen koltuğun numarası koltuğun tag'inde tutulduğundan tag üzerinden direk alınabilir.
            secilenKoltuk = Convert.ToInt32((sender as PictureBox).Tag.ToString());

            cns = new Cinsiyet();
            cns.Show();

            //Herhangi bir koltuk seçildiğinde cinsiyet seçimi yapıldıktan sonra işlemler gerçekleşir.
            cns.btnCinsiyet.Click += BtnCinsiyet_Click;


        }

        private void BtnCinsiyet_Click(object sender, EventArgs e)
        {
            cinsiyet = cns.rdoErkek.Checked;
            KoltuklariDoldur(gidisSeferID);
            PictureBox yanKoltuk;

            //Seçilen koltuğun yan koltuğun bulunup yanda oturan birisi varsa, girilen cinsiyetle aynı olup olmadığına bakılır. Farklıysa uyarı verilir.
            if (secilenKoltuk % 2 == 0)
            {
                int yanKoltukNo = secilenKoltuk - 1;
                string yanKoltukName = secilenPb.Name.Substring(0, 1).ToString() + (yanKoltukNo).ToString();
                yanKoltuk = this.Controls.Find(yanKoltukName, true).FirstOrDefault() as PictureBox;
            }
            else
            {
                int yanKoltukNo = secilenKoltuk + 1;
                string yanKoltukName = secilenPb.Name.Substring(0, 1).ToString() + (yanKoltukNo).ToString();
                yanKoltuk = this.Controls.Find(yanKoltukName, true).FirstOrDefault() as PictureBox;
            }


            //Yan koltuk boş değilse işleme başla.. Burada yan koltukta bayan yada erkek oturması durumu image tag özelliğiyle kontrol altında tutulmuştur. 
            //Bir bayana ait koltuğun image tag'i "b", erkeğe ait koltuğun image tag'i ise "e" olur.
            //Eğer iki bilet aynı kişiye aitse kadın erkek yan yana oturabilir.
            if (yanKoltuk.Image.Tag != null && ayniKullanici == false)
            {                
                    if (cinsiyet == false && yanKoltuk.Image.Tag.ToString() == "e")     //Seçilen cinsiyet kadınsa ve yanında erkek oturuyorsa..
                    {
                        MessageBox.Show("Yanyana oturacak yolcuların cinsiyeti aynı olmalıdır.");
                    }
                    else if (cinsiyet == true && yanKoltuk.Image.Tag.ToString() == "b")     //Seçilen cinsiyet erkekse ve yanında kadın oturuyorsa..
                    {
                        MessageBox.Show("Yanyana oturacak yolcuların cinsiyeti aynı olmalıdır.");
                    }
                
                else   //Cinsiyetler farklı değilse bilet almaya izin verir.
                {
                    if (biletDurum == true)
                        secilenPb.Image = UI.Properties.Resources.seciliSatinAl;
                    else if (biletDurum == false)
                        secilenPb.Image = UI.Properties.Resources.seciliRezerve;
                }
                
            }
            else   //Yan koltuk boşsa kontrol etmeden bilet almaya izin verir.
            {
                if (biletDurum == true)
                    secilenPb.Image = UI.Properties.Resources.seciliSatinAl;
                else if (biletDurum == false)
                    secilenPb.Image = UI.Properties.Resources.seciliRezerve;
            }
        }

        private bool YanKoltukAyniKisiyeMiAit(string koltukNumarasi)        //Şuanda bu metod kullanılmıyor.
        {
            int koltukNumarasi2 = Convert.ToInt32(koltukNumarasi);
            var yanKoltuk = biletRepo.GetAll(x => x.KullaniciID == kullanici.KullaniciID && x.KoltukNo == koltukNumarasi2).ToList();
            if (yanKoltuk != null) return true;
            return false;
        }

        private void KoltuklariDoldur(int seferID)
        {
            //İlgili sefere ait satılmış veya rezerve edilmiş biletleri koltuklara doldur.

            if (biletSinifi == 1)  //Ekonomi
            {
                var alinmisBiletler = biletRepo.GetAll(x => x.SeferID == seferID && x.VagonSinifi == false).ToList();

                foreach (Control koltuk in grpKoltukEkonomi.Controls)
                {
                    if (koltuk is PictureBox && koltuk.Name.StartsWith("kd") == false)      //Koltuk olan pictureboxları al.
                    {
                        ((PictureBox)koltuk).Image = UI.Properties.Resources.bos1;      //Önce tüm koltukları boşalt.
                        ((PictureBox)koltuk).Enabled = true;
                        ((PictureBox)koltuk).Image.Tag = null;
                    }
                }

                foreach (var item in alinmisBiletler)
                {
                    if (item.Cinsiyet == false)  //bayan adına alınan biletlere doluKadin isimli görseli yükle
                    {
                        string pbName = "e" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluKadin;
                        pb.Image.Tag = "b";
                        pb.Enabled = false;
                    }
                    else if (item.Cinsiyet == true)  // erkek adına alınan biletlere doluErkek isimli görseli yükle
                    {
                        string pbName = "e" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluErkek;
                        pb.Image.Tag = "e";
                        pb.Enabled = false;
                    }
                }

            }
            else if (biletSinifi == 2)  //Business || Ekonomiyle ikisinin ayrı ayrı yapılmasının sebebi; ekonomi koltuklarının namei e ile başlar, businessın b ile.
            {
                var alinmisBiletler = biletRepo.GetAll(x => x.SeferID == seferID && x.VagonSinifi == true).ToList();

                foreach (Control koltuk in grpKoltukBusiness.Controls)
                {
                    if (koltuk is PictureBox && koltuk.Name.StartsWith("kd") == false)
                    {
                        ((PictureBox)koltuk).Image = UI.Properties.Resources.bos1;
                        ((PictureBox)koltuk).Enabled = true;
                    }
                }

                foreach (var item in alinmisBiletler)
                {
                    if (item.Cinsiyet == false)  //satın alınan ve bayan
                    {
                        string pbName = "b" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluKadin;
                        pb.Enabled = false;
                    }
                    else if (item.Cinsiyet == true)  //satın alınan ve erkek
                    {
                        string pbName = "b" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluErkek;
                        pb.Enabled = false;
                    }
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
            lblFiyat.Text = fiyat.ToString();

        }

        private void btnSatinAl_Click(object sender, EventArgs e)
        {
            FiyatBelirle();     //Fiyatı belirle. Çocuksa fiyatı düşür, ek hizmetler varsa artır vs.

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

            KoltuklariDoldur(gidisSeferID);     //bilet satıldıktan sonra yeni haliyle koltukları güncelle ve kullanıcı bilgileri kısmını temizle, yeni bilet alma işlemleri için hazırla.
            OrtakMetodlar.Temizle(pnlKisi);

            if (biletDurum == true)
                MessageBox.Show("Bilet satın alma işlemi yapılmıştır. ");
            else
                MessageBox.Show("Bilet rezervasyon işlemi yapılmıştır.");

            yolcuSayisi--;      //Yolcu sayısı 0'a ulaşana kadar azalt, böylece her yolcuya bilet al.
            aktifYolcu++;       //O anda bilet alınan yolcuyu tut.

            if (yolcuSayisi != 0)
            {
                ayniKullanici = true;
                YeniBiletAl();
            }
            else
            {
                ayniKullanici = false; 

                if (koltukGidis == 1)           //Dönüş bileti isteniyorsa tekrar bilet al
                {
                    gidisSeferID = donusSeferID;
                    aktifYolcu = 1;
                    yolcuSayisi = Convert.ToInt32(nmrYolcuSayisi.Value);
                    YeniBiletAl();
                    koltukGidis = 0;
                }
                else
                {
                    if (uyeDegil == true)
                        TrenTab.SelectedIndex = 5 ;
                    else
                    TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
                    BiletleriDoldur();
                }
            }
        }

        public void BiletleriDoldur()
        {
            var biletler = biletRepo.GetAll(x => x.KullaniciID == kullanici.KullaniciID && (x.BiletID.ToString()).StartsWith(txtBiletId.Text)).ToList();
            lstBiletler.Items.Clear();
            foreach (var item in biletler)
            {
                ListViewItem item1 = new ListViewItem(item.BiletID.ToString());
                item1.SubItems.Add(item.Ad);
                item1.SubItems.Add(item.Soyad);
                string cikisDurak = durakRepo.GetAll(x => x.DurakID == item.Sefer.Rota.CikisID).Select(x => x.DurakAdi).SingleOrDefault();
                string varisDurak = durakRepo.GetAll(x => x.DurakID == item.Sefer.Rota.VarisID).Select(x => x.DurakAdi).SingleOrDefault();
                item1.SubItems.Add(cikisDurak);
                item1.SubItems.Add(varisDurak);
                item1.SubItems.Add(item.Sefer.CikisSaati.ToString());
                item1.SubItems.Add(item.SigortaliMi ? "Var" : "Yok");
                item1.SubItems.Add(item.YemekliMi ? "Var" : "Yok");
                item1.SubItems.Add(item.YolculukHizmetiVarMi ? "Var" : "Yok");
                item1.SubItems.Add(item.Fiyat.ToString());

                lstBiletler.Items.Add(item1);

            }

        }

        public void YeniBiletAl()
        {
            KoltukSecimiYolcu.Text = aktifYolcu.ToString();
            KoltuklariDoldur(gidisSeferID);
            OrtakMetodlar.Temizle(pnlKisi);
            rdoYetiskin.Checked = true;
            lblFiyat.Text = "0";
            grpKoltukBusiness.Visible = false;
            grpKoltukEkonomi.Visible = false;
        }

        private void TrenTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TrenTab.SelectedIndex == 3)
            {

            }
        }

        private void btnBiletlerim_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 4;
        }

        private void btnGuvenliCikis_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 0;
        }

        private void btnBiletiAl_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 1;
        }

        private void grpDurak_Enter(object sender, EventArgs e)
        {

        }

        private void btnSorgula_Click(object sender, EventArgs e)
        {
            BiletleriDoldur();
        }
        public bool SeferVarMi(int cikisId, int varisId, DateTime gidisTarihi)      //aranan seferler varmı diye bakar. sefer yoksa kullanıcıya uyarı verir.
        {
            var rotaId = rotaRepo.GetAll(x => x.CikisID == cikisId && x.VarisID == varisId).Select(x => x.RotaID).SingleOrDefault();
            var seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi).ToList();
            if (seferler.Count == 0)
            {
                return false;
            }
            return true;
        }

        private void btnGeriSefer_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 1;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 2;
        }

        private void btnBiletAra_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 5;
        }

        private void btnBiletNoIleAra_Click(object sender, EventArgs e)
        {
            int bNo = int.Parse(txtBiletNumarasi.Text);
            var biletim = biletRepo.GetAll(x => x.BiletID == bNo).SingleOrDefault();

            string cikisDurak = durakRepo.GetAll(x => x.DurakID == biletim.Sefer.Rota.CikisID).Select(x => x.DurakAdi).SingleOrDefault();
            string varisDurak = durakRepo.GetAll(x => x.DurakID == biletim.Sefer.Rota.VarisID).Select(x => x.DurakAdi).SingleOrDefault();

            lblBiletNoSonuc.Text = "Ad: " +biletim.Ad
                +"\nSoyad: "+biletim.Soyad
                +"\nÇıkış Durağı: " + cikisDurak 
                +"\nVarış Durağı: "+ varisDurak
                +"\nÇıkış Saati: " + biletim.Sefer.CikisSaati.ToString()
                +"\nSigorta: " + (biletim.SigortaliMi ? "Var" : "Yok")
                + "\nYemek: " + (biletim.YemekliMi ? "Var" : "Yok")
                + "\nYolculuk Hizmeti: " + (biletim.YolculukHizmetiVarMi ? "Var" : "Yok")
                + "\nÇocuk: " + (biletim.CocukMu ? "Evet" : "Hayır")
                + "\nFiyat: " + (biletim.Fiyat.ToString());

        }

        private void Biletlerim_Click(object sender, EventArgs e)
        {

        }

        public void SeferleriGetir(int cikisId, int varisId, DateTime gidisTarihi)
        {
            var rotaId = rotaRepo.GetAll(x => x.CikisID == cikisId && x.VarisID == varisId).Select(x => x.RotaID).SingleOrDefault();
            var seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi).ToList();

            if (gidisTarihi == DateTime.Now)
            {
                seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && x.CikisSaati.Hours > DateTime.Now.Hour).ToList();
            }

            if (biletDurum == false)
            {     //2 saat kala seferleri rezervasyona kapat
                seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && x.CikisSaati.Hours > DateTime.Now.Hour + 2).ToList();
            }

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
            // lstSeferler.SelectedItems.Clear();

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


