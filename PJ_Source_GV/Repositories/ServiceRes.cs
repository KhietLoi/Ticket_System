using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PJ_Source_GV.Repositories
{
    /// <summary>
    /// Lấy data từ service : thông tin hệ đào tạo, lớp của khoa,đơn vị quản lý SV,......
    /// Create: dinhnn
    /// </summary>
    public class ServiceRes
    {
        /// <summary>
        /// Lấy thông tin hệ đào tạo
        /// Create: dinhnn
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ServiceModel>> LayThongTinHeDaoTao()
        {
            var Service = new ServiceReference.Service1Client();
            ServiceReference.Academic_LayTatCa_SinhResponse sV =
                await Service.Academic_LayTatCa_SinhAsync("75528a06a8dd722c79c01197c04eb2bc");
            List<ServiceModel> listServiceModel_HeDaoTao = new List<ServiceModel>();
            string xml = sV.Academic_LayTatCa_SinhResult.Any1.FirstNode.ToString();
            DataSet ds = new DataSet();
            //Load all xml data in to dataset
            StringReader strReader = new StringReader(xml);
            ds.ReadXml(strReader);

            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var serviceModel = new ServiceModel
                    { 
                        AcademicID = dr[0].ToString(),
                        AcademicName = dr[1].ToString()
                    };
                    listServiceModel_HeDaoTao.Add(serviceModel);
                }
            }

            return listServiceModel_HeDaoTao;
        }

        /// <summary>
        /// Lấy thông tin đơn vị quản lý sinh viên (Cái này khác với đơn vị đăng bài)
        /// Create: dinhnn
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ServiceModel>> LayThongTinDonViQuanLySinhVien()
        {
            var Service = new ServiceReference.Service1Client();
            ServiceReference.Faculty_LayTatCaResponse sV =
                await Service.Faculty_LayTatCaAsync("75528a06a8dd722c79c01197c04eb2bc");
            List<ServiceModel> listServiceModel_DVQLSV = new List<ServiceModel>();
            string xml = sV.Faculty_LayTatCaResult.Any1.FirstNode.ToString();

            DataSet ds = new DataSet();
            //Load all xml data in to dataset
            StringReader strReader = new StringReader(xml);
            ds.ReadXml(strReader);

            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var serviceModel = new ServiceModel
                    {
                        FacultyID = dr[0].ToString(),
                        FacultyName = dr[1].ToString()
                    };
                    listServiceModel_DVQLSV.Add(serviceModel);
                }
            }

            return listServiceModel_DVQLSV;
        }
    }
}
