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
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            var girisYapan = kullaniciRepo.Get(x => x.Email == txtEposta.Text && x.Sifre == x.Sifre);

            if (girisYapan != null)
            {
                Kullanici kullanici = new Kullanici();
                kullanici.KullaniciID = girisYapan.KullaniciID;
                TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
            }
            else
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
        }
    }
}
