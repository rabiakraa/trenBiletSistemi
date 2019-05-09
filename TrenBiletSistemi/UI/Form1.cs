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

        public void DbInitialize()
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
            DbInitialize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            /*Durak durak = new Durak
         {
             DurakAdi = "Erzincan"
         };
         _durakRepository.Add(durak);
         int process = _uow.SaveChanges();*/

            //DELETE
            /*_durakRepository.Delete(6);
             int process = _uow.SaveChanges(); */

            //UPDATE
            /* Durak durak = _durakRepository.GetById(2);
             durak.DurakAdi = "Sakarya";
             _durakRepository.Update(durak);
             int process = _uow.SaveChanges();
             */

            //SELECT

            //var duraklarim = _durakRepository.GetAll(x => x.DurakID == 2).ToList();

            // var duraklarim = _durakRepository.GetAll().ToList();

            /* string duraklar = "";

             foreach (var item in duraklarim)
             {
                 duraklar += item.DurakAdi.ToString() + " ";
             }

             label1.Text = duraklar; */

     

                    ////btn.BackColor = Color.Green;
                    //btn.Enabled = false;
                    //sayac++;
                }


            }


        }
    

