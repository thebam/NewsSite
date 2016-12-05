using System;
using System.Collections.Generic;

namespace NewsSite.Models
{
    public class MediaKitFileViewModel
    {
        private string _iconUrl;
        public int MediaKitFileId { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public string IconURL {
            get { return _iconUrl; }
            set
            {
                _iconUrl = value;

                string[] ext = _iconUrl.Split('.');
                if (ext.Length > 0)
                {
                    switch (ext[ext.Length-1].ToLower())
                    {
                        case "jpg":
                            _iconUrl = "<img class=\"img-responsive\" src=\"/mediakitfiles/" + _iconUrl + "\" />";
                            break;
                        case "gif":
                            _iconUrl = "<img class=\"img-responsive\" src=\"/mediakitfiles/" + _iconUrl + "\" />";
                            break;
                        case "png":
                            _iconUrl = "<img class=\"img-responsive\" src=\"/mediakitfiles/" + _iconUrl + "\" />";
                            break;
                        case "doc":
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/word_icon.png\" /></a>";
                            break;
                        case "docx":
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/word_icon.png\" /></a>";
                            break;
                        case "pdf":
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/pdf_icon.png\" /></a>";
                            break;
                        case "xls":
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/excel_icon.png\" /></a>";
                            break;
                        case "xlsx":
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/excel_icon.png\" /></a>";
                            break;
                        case "ppt":
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/powerpoint_icon.png\" /></a>";
                            break;
                        case "pptx":
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/powerpoint_icon.png\" /></a>";
                            break;
                        default:
                            _iconUrl = "<a href=\"/mediakitfiles/" + _iconUrl + "\"><img class=\"img-responsive\" src=\"/images/file_icon.png\" /></a>";
                            break;
                    }
                }
            }

        }

        public DateTime CopyrightDate { get; set; }
        public List<TagName> TagNames { get; set; }
    }

    public class TagName
    {
        public string Name { get; set; }
    }
}
