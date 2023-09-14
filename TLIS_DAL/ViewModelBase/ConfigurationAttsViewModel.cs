using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class ConfigurationAttsViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        public bool Disable { get; set; }

        
        public ConfigurationAttsViewModel()
        {
        }
        public ConfigurationAttsViewModel(string name)
        {
            Name = name;
        }
        public ConfigurationAttsViewModel(int id,string name)
        {
            Id = id;
            Name = name;
        }
        public ConfigurationAttsViewModel(int id, string name,string tableName,bool disable)
        {
            Id = id;
            Name = name;
            TableName = tableName;
            Disable = disable;
        }

    }

    public class ConfigurationListViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool isDynamic { get; set; }

        public ConfigurationListViewModel()
        {
        }
        public ConfigurationListViewModel(string name , bool isdynamic)
        {
            Name = name;
            isDynamic = isdynamic;
        }
        public ConfigurationListViewModel(int id, string name, bool isdynamic)
        {
            Id = id;
            Name = name;
            isDynamic = isdynamic;
        }

    }

    public class AddConfigrationAttViewModel
    {
        public string Name { get; set; }
        public string TableName { get; set; }

    }

}
