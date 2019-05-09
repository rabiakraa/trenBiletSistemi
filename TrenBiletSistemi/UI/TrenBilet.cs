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
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            var girisYapan = kullaniciRepo.Get(x => x.Email == txtEposta.Text && x.Sifre == txtSifre.Text);

            if (girisYapan != null)
            {
                Kullanici kullanici = new Kullanici();
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

                if (cmbNereden.Text!= itemCmb.Text)
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
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            lstSeferler.Items.Clear();

            if (OrtakMetodlar.BosAlanVarMi(grpDurak))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
            else
            {
                int cikisId = Convert.ToInt32(((ComboboxItem)cmbNereden.SelectedItem).Value);
                int varisId = Convert.ToInt32(((ComboboxItem)cmbNereye.SelectedItem).Value);
                DateTime gidisTarihi = dtGidis.Value;

                var rotaId = rotaRepo.GetAll(x => x.CikisID == cikisId && x.VarisID == varisId).Select(x=> x.RotaID).SingleOrDefault();

                var seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi).ToList();

                foreach (var item in seferler)
                {
                    ListViewItem item1 = new ListViewItem(item.SeferID.ToString());
                    //  item1.SubItems.Add(item.Rota.CikisID.ToString());
                    // item1.SubItems.Add(durakRepo.GetAll(x => x.DurakID  ==  item.Rota.CikisID).ToString());
                    item1.SubItems.Add(cmbNereden.Text);
                    item1.SubItems.Add(cmbNereye.Text);
                    item1.SubItems.Add(item.CikisSaati.ToString());
                    item1.SubItems.Add(item.VarisSaati.ToString());
                    item1.SubItems.Add(item.Tarih.ToString());
                    item1.SubItems.Add(item.SeferSuresi.ToString());
                    lstSeferler.Items.Add(item1);

                }
                TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;

            }
        }

    }

}
    

