use crate::setting_value::SettingValue;

#[cfg(target_pointer_width = "64")]
pub use livesplit_auto_splitting::settings::List as SettingsList;

#[cfg(not(target_pointer_width = "64"))]
pub type SettingsList = ();

#[no_mangle]
pub extern "C" fn SettingsList_new() -> Box<SettingsList> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingsList::new())
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

#[no_mangle]
pub extern "C" fn SettingsList_drop(_: Box<SettingsList>) {}

#[no_mangle]
pub extern "C" fn SettingsList_push(_this: &mut SettingsList, _value: Box<SettingValue>) {
    #[cfg(target_pointer_width = "64")]
    {
        _this.push(*_value);
    }
}

#[no_mangle]
pub extern "C" fn SettingsList_len(_this: &SettingsList) -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        _this.len()
    }
    #[cfg(not(target_pointer_width = "64"))]
    0
}

#[no_mangle]
pub extern "C" fn SettingsList_get(_this: &SettingsList, _index: usize) -> &SettingValue {
    #[cfg(target_pointer_width = "64")]
    {
        _this.get(_index).unwrap()
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}
