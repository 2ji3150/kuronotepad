using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace kuronotepad {
    class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly PropertyChangedEventArgs UTF8PropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(UTF8));

        private bool _utf8;
        public bool UTF8 {
            get => _utf8;
            set {
                if (_utf8 == value) return;
                _utf8 = value;
                PropertyChanged?.Invoke(this, UTF8PropertyChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs TitlePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Title));

        private string _title = "無題 - クロノメモ帳";
        public string Title {
            get => _title;
            set {
                if (_title == value) return;
                _title = value;
                PropertyChanged?.Invoke(this, TitlePropertyChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs CaretPositionPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(CaretPosition));

        private string _caretposition = "1 行    1 列";
        public string CaretPosition {
            get => _caretposition;
            set {
                if (_caretposition == value) return;
                _caretposition = value;
                PropertyChanged?.Invoke(this, CaretPositionPropertyChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));

        private bool _isdirty = false;
        public bool IsDirty {
            get => _isdirty;
            set {
                if (_isdirty == value) return;
                _isdirty = value;
                PropertyChanged?.Invoke(this, IsDirtyPropertyChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs EncodePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Encode));

        private string _encode = "ShiftJIS";
        public string Encode {
            get => _encode;
            set {
                _encode = value;
                PropertyChanged?.Invoke(this, EncodePropertyChangedEventArgs);
            }
        }
    }
}
