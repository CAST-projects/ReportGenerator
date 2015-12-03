using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ComponentRepository;

namespace TaskPaneComponents
{
    public class ComponentsListViewModel: ViewModelBase
    {
        public event ComponentViewModelDragEventHandler ReportDragEvent;

        #region Fields

        List<ComponentViewModel> componentsList;
        ObservableCollection<ComponentViewModel> components;
        string searchText;
        ComponentType selectedType;
        CommandViewModel clearSearchTextCommand;
        ComponentsRepository componentsRepository;

        #endregion

        #region Constructor

        public ComponentsListViewModel()
        {
            searchText = null;
            SelectedType = ComponentType.All;

            this.PropertyChanged += ComponentsListViewModel_PropertyChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<ComponentViewModel> Components {
            get { return components; }
            private set
            {
                if (value == components)
                    return;

                components = value;
                base.OnPropertyChanged(Consts.COMPONENTS_PN);
            }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (value == searchText)
                    return;

                searchText = value;
                base.OnPropertyChanged(Consts.SEARCH_TEXT_PN);
            }
        }

        public ComponentType SelectedType
        {
            get { return selectedType; }
            set
            {
                if (value == selectedType)
                    return;

                selectedType = value;
                base.OnPropertyChanged(Consts.SELECTEDTYPE_PN);
            }
        }

        public CommandViewModel ClearSearchTextCommand
        {
            get
            {
                if (clearSearchTextCommand == null)
                    clearSearchTextCommand = new CommandViewModel(new RelayCommand(param => this.ClearSearchText(), (obj) => !string.IsNullOrEmpty(SearchText)));

                return clearSearchTextCommand;
            }
        }

        private void ClearSearchText()
        {
            SearchText = null;
        }

        #endregion

        #region Public Methods

        public void CreateAllComponents(ComponentsRepository componentsRepository)
        {
            this.componentsRepository = componentsRepository;

            componentsList = new List<ComponentViewModel>();

            foreach (Component component in componentsRepository.Components)
            {
                ComponentViewModel compVM = new ComponentViewModel(component);
                compVM.ReportDragEvent += compVM_ReportDragEvent;

                componentsList.Add(compVM);
            }

            Components = new ObservableCollection<ComponentViewModel>(componentsList);
        }
       
        #endregion

        #region Event Handlers

        void compVM_ReportDragEvent(object sender, DragEventArgs e)
        {
            ComponentViewModelDragEventHandler handler = this.ReportDragEvent;
            if (handler != null)
                handler(this, e);
        }

        void ComponentsListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Consts.SEARCH_TEXT_PN:
                case Consts.SELECTEDTYPE_PN: Components = filterComponent(); break;
            }
        }

        private ObservableCollection<ComponentViewModel> filterComponent()
        {
            List<ComponentViewModel> list = componentsList;
            if (SelectedType != ComponentType.All)
                list = componentsList.Where(x => x.CompoType == SelectedType).ToList();

            if (!string.IsNullOrEmpty(SearchText))
                    list = list.Where(e => e.DisplayName.ToLower().StartsWith(SearchText.ToLower())).ToList();

            return new ObservableCollection<ComponentViewModel>(list);
        }

        #endregion

        protected override void OnDispose()
        {
            if (componentsRepository != null)
            {
                componentsRepository.Dispose();
                componentsRepository = null;
            }
        }
    }
}
