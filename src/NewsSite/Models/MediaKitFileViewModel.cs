using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class MediaKitFileViewModel : MediaKitFile
    {
        private string _iconUrl;
        public string IconURL {
            get { return _iconUrl; }
            set
            {
                _iconUrl = value;

                string URL = "";
                //URL = "/ssu/news";
                URL = "";


                string[] ext = _iconUrl.Split('.');
                if (ext.Length > 0)
                {
                    switch (ext[ext.Length-1].ToLower())
                    {
                        case "jpg":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" /></a>";
                            break;
                        case "gif":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" /></a>";
                            break;
                        case "png":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" /></a>";
                            break;
                        case "doc":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/word_icon.png\" /></a>";
                            break;
                        case "docx":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/word_icon.png\" /></a>";
                            break;
                        case "pdf":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/pdf_icon.png\" /></a>";
                            break;
                        case "xls":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/excel_icon.png\" /></a>";
                            break;
                        case "xlsx":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/excel_icon.png\" /></a>";
                            break;
                        case "ppt":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/powerpoint_icon.png\" /></a>";
                            break;
                        case "pptx":
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/powerpoint_icon.png\" /></a>";
                            break;
                        default:
                            _iconUrl = "<a href=\"" + URL + "/mediakitfiles/" + _iconUrl + "\" target=\"_blank\"><img class=\"img-responsive\" src=\"" + URL + "/images/file_icon.png\" /></a>";
                            break;
                    }
                }
            }

        }
        
        public List<TagName> TagNames { get; set; }
    }

    public class TagName
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
