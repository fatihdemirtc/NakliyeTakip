using CommunityToolkit.Mvvm.Input;
using NakliyeTakip.MAUI.Models;

namespace NakliyeTakip.MAUI.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}