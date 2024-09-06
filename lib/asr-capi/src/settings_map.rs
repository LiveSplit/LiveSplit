use crate::setting_value::SettingValue;

#[cfg(target_pointer_width = "64")]
use crate::{output_str, str};

#[cfg(target_pointer_width = "64")]
pub use livesplit_auto_splitting::settings::Map as SettingsMap;

#[cfg(not(target_pointer_width = "64"))]
pub type SettingsMap = ();

#[no_mangle]
pub extern "C" fn SettingsMap_new() -> Box<SettingsMap> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingsMap::new())
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

#[no_mangle]
pub extern "C" fn SettingsMap_drop(_: Box<SettingsMap>) {}

/// # Safety
/// TODO:
#[no_mangle]
pub unsafe extern "C" fn SettingsMap_insert(
    _this: &mut SettingsMap,
    _key_ptr: *const u8,
    _value: Box<SettingValue>,
) {
    #[cfg(target_pointer_width = "64")]
    {
        _this.insert(str(_key_ptr).into(), *_value);
    }
}

#[no_mangle]
pub extern "C" fn SettingsMap_len(_this: &SettingsMap) -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        _this.len()
    }
    #[cfg(not(target_pointer_width = "64"))]
    0
}

#[no_mangle]
pub extern "C" fn SettingsMap_get_key(_this: &SettingsMap, _index: usize) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        output_str(_this.get_by_index(_index).unwrap().0)
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn SettingsMap_get_value(_this: &SettingsMap, _index: usize) -> &SettingValue {
    #[cfg(target_pointer_width = "64")]
    {
        _this.get_by_index(_index).unwrap().1
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub unsafe extern "C" fn SettingsMap_get_value_by_key(
    _this: &SettingsMap,
    _key_ptr: *const u8,
) -> Option<&SettingValue> {
    #[cfg(target_pointer_width = "64")]
    {
        _this.get(str(_key_ptr))
    }
    #[cfg(not(target_pointer_width = "64"))]
    None
}
