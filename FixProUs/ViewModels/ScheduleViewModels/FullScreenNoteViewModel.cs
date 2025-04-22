using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;


namespace FixProUs.ViewModels
{
    public partial class FullScreenNoteViewModel : BaseViewModel
    {
        [ObservableProperty]
        string note;

        public delegate void NoteDelegte(string Note);

        public event NoteDelegte NoteClose;

        public FullScreenNoteViewModel()
        {
            
        }

        public FullScreenNoteViewModel(string note)
        {
            Note = note;  
        }

        [RelayCommand]
        async void ApplyNote(string note)
        {
            IsEnable = false;

            NoteClose.Invoke(note);
           
            await App.Current!.MainPage!.Navigation.PopAsync();
            IsEnable = true;
        }


    }
}
