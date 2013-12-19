using Caliburn.Micro;
using Dev2.Activities.Designers2.Core;
using Dev2.Data.Parsers;
using Dev2.Data.Util;
using Dev2.DataList.Contract;
using Dev2.DynamicServices;
using Dev2.Providers.Errors;
using Dev2.Providers.Logs;
using Dev2.Runtime.Configuration.ViewModels.Base;
using Dev2.Runtime.ServiceModel.Data;
using Dev2.Services.Events;
using Dev2.Studio.Core;
using Dev2.Studio.Core.Activities.Utils;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Core.Messages;
using Dev2.Threading;
using Dev2.TO;
using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Dev2.Activities.Designers2.SqlBulkInsert
{
    public class SqlBulkInsertDesignerViewModel : ActivityCollectionDesignerViewModel<DataColumnMapping>
    {
        readonly IEventAggregator _eventPublisher;
        readonly IEnvironmentModel _environmentModel;
        readonly IAsyncWorker _asyncWorker;

        bool _isInitializing;

        static readonly DbTableList EmptyDbTables = new DbTableList();
        static readonly DbColumnList EmptyDbColumns = new DbColumnList();
        static readonly DbSource NewDbSource = new DbSource
        {
            ResourceID = Guid.NewGuid(),
            ResourceName = "New Database Source..."
        };
        static readonly DbSource SelectDbSource = new DbSource
        {
            ResourceID = Guid.NewGuid(),
            ResourceName = "Select a Database..."
        };
        static readonly DbTable SelectDbTable = new DbTable
        {
            TableName = "Select a Table..."
        };

        public SqlBulkInsertDesignerViewModel(ModelItem modelItem)
            : this(modelItem, new AsyncWorker(), EnvironmentRepository.Instance.ActiveEnvironment, EventPublishers.Aggregator)
        {
        }

        public SqlBulkInsertDesignerViewModel(ModelItem modelItem, IAsyncWorker asyncWorker, IEnvironmentModel environmentModel, IEventAggregator eventPublisher)
            : base(modelItem)
        {
            VerifyArgument.IsNotNull("asyncWorker", asyncWorker);
            _asyncWorker = asyncWorker;
            VerifyArgument.IsNotNull("environmentModel", environmentModel);
            _environmentModel = environmentModel;
            VerifyArgument.IsNotNull("eventPublisher", eventPublisher);
            _eventPublisher = eventPublisher;

            AddTitleBarLargeToggle();
            AddTitleBarQuickVariableInputToggle();
            AddTitleBarHelpToggle();

            dynamic mi = ModelItem;
            ModelItemCollection = mi.InputMappings;

            Databases = new ObservableCollection<DbSource>();
            Tables = new ObservableCollection<DbTable>();

            EditDatabaseCommand = new RelayCommand(o => EditDbSource(), o => IsDatabaseSelected);
            RefreshTablesCommand = new RelayCommand(o => RefreshTables(), o => IsTableSelected);

            RefreshDatabases(true);
        }

        #region Properties

        public override string CollectionName { get { return "InputMappings"; } }

        public ObservableCollection<DbSource> Databases { get; private set; }

        public ObservableCollection<DbTable> Tables { get; private set; }

        public ICommand EditDatabaseCommand { get; private set; }

        public ICommand RefreshTablesCommand { get; private set; }

        public bool IsDatabaseSelected { get { return SelectedDatabase != SelectDbSource; } }

        public bool IsTableSelected { get { return SelectedTable != SelectDbTable; } }

        public bool IsRefreshing { get { return (bool)GetValue(IsRefreshingProperty); } set { SetValue(IsRefreshingProperty, value); } }

        public static readonly DependencyProperty IsRefreshingProperty =
            DependencyProperty.Register("IsRefreshing", typeof(bool), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(false));

        public DbSource SelectedDatabase { get { return (DbSource)GetValue(SelectedDatabaseProperty); } set { SetValue(SelectedDatabaseProperty, value); } }

        public static readonly DependencyProperty SelectedDatabaseProperty =
            DependencyProperty.Register("SelectedDatabase", typeof(DbSource), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(null, OnSelectedDatabaseChanged));

        static void OnSelectedDatabaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SqlBulkInsertDesignerViewModel)d;
            if(viewModel.IsRefreshing)
            {
                return;
            }
            viewModel.OnSelectedDatabaseChanged();
        }

        public DbTable SelectedTable { get { return (DbTable)GetValue(SelectedTableProperty); } set { SetValue(SelectedTableProperty, value); } }

        public static readonly DependencyProperty SelectedTableProperty =
            DependencyProperty.Register("SelectedTable", typeof(DbTable), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(null, OnSelectedTableChanged));

        static void OnSelectedTableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SqlBulkInsertDesignerViewModel)d;
            if(viewModel.IsRefreshing)
            {
                return;
            }
            viewModel.OnSelectedTableChanged();
        }

        public bool IsSelectedDatabaseFocused { get { return (bool)GetValue(IsSelectedDatabaseFocusedProperty); } set { SetValue(IsSelectedDatabaseFocusedProperty, value); } }

        public static readonly DependencyProperty IsSelectedDatabaseFocusedProperty =
            DependencyProperty.Register("IsSelectedDatabaseFocused", typeof(bool), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(false));

        public bool IsSelectedTableFocused { get { return (bool)GetValue(IsSelectedTableFocusedProperty); } set { SetValue(IsSelectedTableFocusedProperty, value); } }

        public static readonly DependencyProperty IsSelectedTableFocusedProperty =
            DependencyProperty.Register("IsSelectedTableFocused", typeof(bool), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(false));

        public bool IsBatchSizeFocused { get { return (bool)GetValue(IsBatchSizeFocusedProperty); } set { SetValue(IsBatchSizeFocusedProperty, value); } }

        public static readonly DependencyProperty IsBatchSizeFocusedProperty =
            DependencyProperty.Register("IsBatchSizeFocused", typeof(bool), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(false));

        public bool IsTimeoutFocused { get { return (bool)GetValue(IsTimeoutFocusedProperty); } set { SetValue(IsTimeoutFocusedProperty, value); } }

        public static readonly DependencyProperty IsTimeoutFocusedProperty =
            DependencyProperty.Register("IsTimeoutFocused", typeof(bool), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(false));

        public bool IsInputMappingsFocused
        {
            get { return (bool)GetValue(IsInputMappingsFocusedProperty); }
            set { SetValue(IsInputMappingsFocusedProperty, value); }
        }

        public static readonly DependencyProperty IsInputMappingsFocusedProperty =
            DependencyProperty.Register("IsInputMappingsFocused", typeof(bool), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(false));

        public bool IsResultFocused
        {
            get { return (bool)GetValue(IsResultFocusedProperty); }
            set { SetValue(IsResultFocusedProperty, value); }
        }

        public static readonly DependencyProperty IsResultFocusedProperty =
            DependencyProperty.Register("IsResultFocused", typeof(bool), typeof(SqlBulkInsertDesignerViewModel), new PropertyMetadata(false));

        #endregion

        #region DO NOT bind to these properties - these are here for internal view model use only!!!

        DbSource Database
        {
            get { return GetProperty<DbSource>(); }
            set
            {
                if(!_isInitializing)
                {
                    SetProperty(value);
                }
            }
        }

        string TableName
        {
            get { return GetProperty<string>(); }
            set
            {
                if(!_isInitializing)
                {
                    SetProperty(value);
                }
            }
        }

        string BatchSize { get { return GetProperty<string>(); } }

        string Timeout { get { return GetProperty<string>(); } }

        string Result { get { return GetProperty<string>(); } }

        #endregion

        protected virtual void OnSelectedDatabaseChanged()
        {
            if(SelectedDatabase == NewDbSource)
            {
                CreateDbSource();
                return;
            }

            IsRefreshing = true;

            // Save selection
            var selectedTableName = GetTableName(SelectedTable);

            Databases.Remove(SelectDbSource);
            Database = SelectedDatabase;

            Tables.Clear();
            LoadDatabaseTables(() =>
            {
                // Restore Selection
                SetSelectedTable(selectedTableName);
                LoadTableColumns(() => { IsRefreshing = false; });
            });
        }

        protected virtual void OnSelectedTableChanged()
        {
            if(SelectedTable != null)
            {
                IsRefreshing = true;
                Tables.Remove(SelectDbTable);
                TableName = SelectedTable.TableName;
                LoadTableColumns(() => { IsRefreshing = false; });
            }
        }

        void RefreshDatabases(bool isInitializing = false)
        {
            IsRefreshing = true;
            if(isInitializing)
            {
                _isInitializing = true;
            }

            LoadDatabases(() =>
            {
                SetSelectedDatabase(Database);
                LoadDatabaseTables(() =>
                {
                    SetSelectedTable(TableName);
                    LoadTableColumns(() =>
                    {
                        IsRefreshing = false;
                        if(isInitializing)
                        {
                            _isInitializing = false;
                        }
                    });
                });
            });
        }

        void RefreshTables()
        {
            IsRefreshing = true;

            LoadDatabaseTables(() =>
            {
                SetSelectedTable(TableName);
                LoadTableColumns(() =>
                {
                    IsRefreshing = false;
                });
            });
        }

        void LoadDatabases(System.Action continueWith = null)
        {
            Databases.Clear();
            Databases.Add(NewDbSource);

            _asyncWorker.Start(() => GetDatabases().OrderBy(r => r.ResourceName), databases =>
            {
                foreach(var database in databases)
                {
                    Databases.Add(database);
                }
                if(continueWith != null)
                {
                    continueWith();
                }
            });
        }

        void LoadDatabaseTables(System.Action continueWith = null)
        {
            Tables.Clear();

            if(!IsDatabaseSelected)
            {
                if(continueWith != null)
                {
                    continueWith();
                }
                return;
            }

            // Get Selected values on UI thread BEFORE starting asyncWorker
            var selectedDatabase = SelectedDatabase;
            _asyncWorker.Start(() => GetDatabaseTables(selectedDatabase), tableList =>
            {
                if(tableList.HasErrors)
                {
                    Errors = new List<IActionableErrorInfo>
                    {
                        new ActionableErrorInfo(() => IsSelectedTableFocused = true) { ErrorType = ErrorType.Critical, Message = tableList.Errors }
                    };
                }
                else
                {
                    Errors = null;
                }
                foreach(var table in tableList.Items.OrderBy(t => t.TableName))
                {
                    Tables.Add(table);
                }
                if(continueWith != null)
                {
                    continueWith();
                }
            });
        }

        void LoadTableColumns(System.Action continueWith = null)
        {
            if(!IsTableSelected || _isInitializing)
            {
                if(!_isInitializing)
                {
                    ModelItemCollection.Clear();
                }
                if(continueWith != null)
                {
                    continueWith();
                }
                return;
            }

            var oldColumns = GetInputMappings().ToList();
            ModelItemCollection.Clear();

            // Get Selected values on UI thread BEFORE starting asyncWorker
            var selectedDatabase = SelectedDatabase;
            var selectedTable = SelectedTable;
            _asyncWorker.Start(() => GetDatabaseTableColumns(selectedDatabase, selectedTable), columnList =>
            {
                if(columnList.HasErrors)
                {
                    Errors = new List<IActionableErrorInfo>
                    {
                        new ActionableErrorInfo(() => IsSelectedTableFocused = true) { ErrorType = ErrorType.Critical, Message = columnList.Errors }
                    };
                }
                else
                {
                    Errors = null;
                }
                foreach(var mapping in columnList.Items.Select(column => new DataColumnMapping { OutputColumn = column }))
                {
                    var oldColumn = oldColumns.FirstOrDefault(c => c.OutputColumn.ColumnName == mapping.OutputColumn.ColumnName);
                    if(oldColumn != null)
                    {
                        mapping.InputColumn = oldColumn.InputColumn;
                    }
                    if(string.IsNullOrEmpty(mapping.InputColumn))
                    {
                        mapping.InputColumn = string.Format("[[{0}(*).{1}]]", selectedTable.TableName, mapping.OutputColumn.ColumnName);
                    }

                    ModelItemCollection.Add(mapping);
                }
                if(continueWith != null)
                {
                    continueWith();
                }
            });
        }

        void EditDbSource()
        {
            var resourceModel = _environmentModel.ResourceRepository.FindSingle(c => c.ResourceName == SelectedDatabase.ResourceName);
            if(resourceModel != null)
            {
                Logger.TraceInfo("Publish message of type - " + typeof(ShowEditResourceWizardMessage));
                _eventPublisher.Publish(new ShowEditResourceWizardMessage(resourceModel));
                RefreshDatabases();
            }
        }

        void CreateDbSource()
        {
            Logger.TraceInfo("Publish message of type - " + typeof(ShowNewResourceWizard));
            _eventPublisher.Publish(new ShowNewResourceWizard("DbSource"));
            RefreshDatabases();
        }

        IEnumerable<DbSource> GetDatabases()
        {
            return _environmentModel.ResourceRepository.FindSourcesByType<DbSource>(_environmentModel, enSourceType.SqlDatabase);
        }

        DbTableList GetDatabaseTables(DbSource dbSource)
        {
            var tables = _environmentModel.ResourceRepository.GetDatabaseTables(dbSource);
            return tables ?? EmptyDbTables;
        }

        DbColumnList GetDatabaseTableColumns(DbSource dbSource, DbTable dbTable)
        {
            var columns = _environmentModel.ResourceRepository.GetDatabaseTableColumns(dbSource, dbTable);
            return columns ?? EmptyDbColumns;
        }

        void SetSelectedDatabase(DbSource dbSource)
        {
            var selectedDatabase = dbSource == null ? null : Databases.FirstOrDefault(d => d.ResourceID == dbSource.ResourceID);
            if(selectedDatabase == null)
            {
                if(Databases.FirstOrDefault(d => d.Equals(SelectDbSource)) == null)
                {
                    Databases.Insert(0, SelectDbSource);
                }
                selectedDatabase = SelectDbSource;
            }
            SelectedDatabase = selectedDatabase;
        }

        void SetSelectedTable(string tableName)
        {
            var selectedTable = Tables.FirstOrDefault(t => t.TableName == tableName);
            if(selectedTable == null)
            {
                if(Tables.FirstOrDefault(t => t.Equals(SelectDbTable)) == null)
                {
                    Tables.Insert(0, SelectDbTable);
                }
                selectedTable = SelectDbTable;
            }
            SelectedTable = selectedTable;
        }

        static string GetTableName(DbTable table)
        {
            return table == null ? null : table.TableName;
        }

        public override void Validate()
        {
            base.Validate();

            var errors = new List<IActionableErrorInfo>();
            if(Errors != null)
            {
                errors.AddRange(Errors);
            }
            errors.AddRange(ValidateValues());
            errors.AddRange(ValidateVariables());

            Errors = errors.Count == 0 ? null : errors;
        }

        IEnumerable<IActionableErrorInfo> ValidateValues()
        {
            if(!IsDatabaseSelected)
            {
                yield return new ActionableErrorInfo(() => IsSelectedDatabaseFocused = true) { ErrorType = ErrorType.Critical, Message = "A database must be selected." };
            }
            if(!IsTableSelected)
            {
                yield return new ActionableErrorInfo(() => IsSelectedTableFocused = true) { ErrorType = ErrorType.Critical, Message = "A table must be selected." };
            }

            var batchSize = BatchSize;
            if(!IsVariable(batchSize))
            {
                int value;
                if(!int.TryParse(batchSize, out value) || value < 0)
                {
                    yield return new ActionableErrorInfo(() => IsBatchSizeFocused = true) { ErrorType = ErrorType.Critical, Message = "Batch size must be a number greater than or equal to zero." };
                }
            }

            var timeout = Timeout;
            if(!IsVariable(timeout))
            {
                int value;
                if(!int.TryParse(timeout, out value) || value < 0)
                {
                    yield return new ActionableErrorInfo(() => IsTimeoutFocused = true) { ErrorType = ErrorType.Critical, Message = "Timeout must be a number greater than or equal to zero." };
                }
            }

            var nonEmptyCount = ModelItemCollection.Count(mi => !string.IsNullOrEmpty(((DataColumnMapping)mi.GetCurrentValue()).InputColumn));
            if(nonEmptyCount == 0)
            {
                yield return new ActionableErrorInfo(() => IsInputMappingsFocused = true) { ErrorType = ErrorType.Critical, Message = "At least one input mapping must be provided." };
            }
        }

        IEnumerable<IActionableErrorInfo> ValidateVariables()
        {
            var parser = new Dev2DataLanguageParser();

            var error = ValidateVariable(parser, BatchSize, () => IsBatchSizeFocused = true);
            if(error != null)
            {
                error.Message = "Batch Size " + error.Message;
                yield return error;
            }

            error = ValidateVariable(parser, Timeout, () => IsTimeoutFocused = true);
            if(error != null)
            {
                error.Message = "Timeout " + error.Message;
                yield return error;
            }

            error = ValidateVariable(parser, Result, () => IsResultFocused = true);
            if(error != null)
            {
                error.Message = "Result " + error.Message;
                yield return error;
            }

            foreach(DataColumnMapping dc in GetInputMappings())
            {
                error = ValidateVariable(parser, dc.InputColumn, () => IsInputMappingsFocused = true);
                if(error != null)
                {
                    error.Message = "Input Mapping To Field '" + dc.OutputColumn.ColumnName + "' " + error.Message;
                    yield return error;
                }
            }
        }

        static IActionableErrorInfo ValidateVariable(Dev2DataLanguageParser parser, string variable, System.Action focusAction)
        {
            if(!string.IsNullOrEmpty(variable))
            {
                try
                {
                    parser.MakeParts(variable);
                }
                catch(Exception ex)
                {
                    return new ActionableErrorInfo(focusAction) { ErrorType = ErrorType.Critical, Message = ex.Message };
                }
            }
            return null;
        }

        static bool IsVariable(string variable)
        {
            var regions = DataListCleaningUtils.SplitIntoRegions(variable).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return regions.Count > 0;
        }

        IEnumerable<DataColumnMapping> GetInputMappings()
        {
            return ModelItemCollection.Select(mi => (DataColumnMapping)mi.GetCurrentValue());
        }

        protected override void AddToCollection(IEnumerable<string> source, bool overWrite)
        {
            var newMappings = source.ToList();
            var max = Math.Min(newMappings.Count, ItemCount);
            for(var i = 0; i < max; i++)
            {
                var mi = ModelItemCollection[i];
                mi.SetProperty("InputColumn", newMappings[i]);
            }
        }

        protected override void OnToggleCheckedChanged(string propertyName, bool isChecked)
        {
            base.OnToggleCheckedChanged(propertyName, isChecked);

            if(propertyName == ShowQuickVariableInputProperty.Name)
            {
                if(!ShowQuickVariableInput)
                {
                    return;
                }

                var mappings = GetInputMappings().ToList();
                QuickVariableInputViewModel.Overwrite = true;
                QuickVariableInputViewModel.IsOverwriteEnabled = false;
                QuickVariableInputViewModel.RemoveEmptyEntries = false;
                QuickVariableInputViewModel.SplitType = Core.QuickVariableInput.QuickVariableInputViewModel.SplitTypeNewLine;
                QuickVariableInputViewModel.VariableListString = string.Join(Environment.NewLine, mappings.Select(GetFieldName));
                QuickVariableInputViewModel.Prefix = GetRecordsetName(mappings) + "(*).";
            }
        }

        static string GetFieldName(DataColumnMapping dc)
        {
            return string.IsNullOrEmpty(dc.InputColumn) ? string.Empty : DataListUtil.ExtractFieldNameFromValue(dc.InputColumn);
        }

        string GetRecordsetName(IEnumerable<DataColumnMapping> mappings)
        {
            var rsName = mappings.Select(m => DataListUtil.ExtractRecordsetNameFromValue(m.InputColumn)).FirstOrDefault(rs => !string.IsNullOrEmpty(rs));
            return string.IsNullOrEmpty(rsName) ? TableName : rsName;
        }
    }
}