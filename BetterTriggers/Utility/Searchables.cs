using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public class Searchable
    {
        public object Object;
        public string Category;
        public List<string> Words;

        public bool HasWord(string word)
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].Contains(word))
                    return true;
            }
            return false;
        }
    }

    public class Searchables: ISearchables
    {
        private List<ISearchablesObserverList> observerList = new List<ISearchablesObserverList>();
        private List<ISearchablesObserverCategories> observerCategories = new List<ISearchablesObserverCategories>();
        private List<Searchable> ItemsAll;
        private List<Searchable> ItemsFiltered;
        private HashSet<string> Categories;
        private string currentWord;
        private string currentCategory = "-1";

        public Searchables(List<Searchable> searchObjects)
        {
            ItemsAll = searchObjects;
            ItemsFiltered = searchObjects;
            Categories = new HashSet<string>();
            Categories.Add("-1");
            for (int i = 0; i < ItemsAll.Count; i++)
            {
                Categories.Add(ItemsAll[i].Category);
            }
        }

        public void Search(string word)
        {
            currentWord = word.ToLower();
            ItemsFiltered = new List<Searchable>();
            Categories = new HashSet<string>();

            if (currentWord == "")
            {
                ItemsFiltered = ItemsAll;
                for (int i = 0; i < ItemsAll.Count; i++)
                {
                    Categories.Add(ItemsAll[i].Category);
                }
                NotifyList();
                NotifyCategories();

                return;
            }

            for (int i = 0; i < ItemsAll.Count; i++)
            {
                var item = ItemsAll[i];
                if(item.HasWord(currentWord))
                {
                    Categories.Add(item.Category);
                    ItemsFiltered.Add(item);
                }
            }

            NotifyList();
            NotifyCategories();
        }

        /// <summary>
        /// Returns a list of objects with the given category id.
        /// It only iterates over objects found in the last search.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<Searchable> GetObjects()
        {
            List<Searchable> list = new List<Searchable>();
            if (currentCategory == "-1")
                return ItemsFiltered;

            for (int i = 0; i < ItemsFiltered.Count; i++)
            {
                if (ItemsFiltered[i].Category == currentCategory)
                    list.Add(ItemsFiltered[i]);
            }

            return list;
        }

        /// <summary>
        /// Returns all objects, regardless of the search input.
        /// </summary>
        public List<Searchable> GetAllObject()
        {
            return ItemsAll;
        }

        /// <summary>
        /// Returns a list of categories found with the last search.
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentCategories()
        {
            List<string> categories = new List<string>();
            foreach (var item in Categories)
            {
                categories.Add(item);
            }
            return categories;
        }

        public void SetCurrentCategory(string categoryId)
        {
            this.currentCategory = categoryId;
            NotifyList();
        }

        public void AttachList(ISearchablesObserverList list)
        {
            observerList.Add(list);
        }

        public void DetachList(ISearchablesObserverList list)
        {
            observerList.Remove(list);
        }

        public void AttachCategories(ISearchablesObserverCategories categories)
        {
            observerCategories.Add(categories);
        }

        public void DetachCategories(ISearchablesObserverCategories categories)
        {
            observerCategories.Remove(categories);
        }

        public void NotifyList()
        {
            for(int i = 0; i < observerList.Count; i++)
            {
                observerList[i].Update();
            }
        }

        public void NotifyCategories()
        {
            for (int i = 0; i < observerCategories.Count; i++)
            {
                observerCategories[i].Update();
            }
        }
    }
}
