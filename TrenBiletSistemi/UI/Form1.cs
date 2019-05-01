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
    public partial class Form1 : Form
    {
        private Context _dbContext;
        private IUnitOfWork _uow;
        private IRepository<Durak> _durakRepository;

        public void TestInitialize()
        {
            _dbContext = new Context();
            // EFBlogContext'i kullanıyor olduğumuz için EFUnitOfWork'den türeterek constructor'ına
            // ilgili context'i constructor injection yöntemi ile inject ediyoruz.
            _uow = new EFUnitOfWork(_dbContext);
            _durakRepository = new EFRepository<Durak>(_dbContext);
        }
        public Form1()
        {
            InitializeComponent();
            TestInitialize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Durak durak = new Durak
            {
                DurakAdi = "İstanbul"
            };
            _durakRepository.Add(durak);
            int process = _uow.SaveChanges();
        }
    }
}
