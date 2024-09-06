use crate::{settings_list::SettingsList, settings_map::SettingsMap};

#[cfg(target_pointer_width = "64")]
use crate::{output_str, str};

#[cfg(target_pointer_width = "64")]
pub use livesplit_auto_splitting::settings::Value as SettingValue;

#[cfg(not(target_pointer_width = "64"))]
pub type SettingValue = ();

#[no_mangle]
pub extern "C" fn SettingValue_new_map(_map: Box<SettingsMap>) -> Box<SettingValue> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingValue::Map(*_map))
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

#[no_mangle]
pub extern "C" fn SettingValue_new_list(_list: Box<SettingsList>) -> Box<SettingValue> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingValue::List(*_list))
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

#[no_mangle]
pub extern "C" fn SettingValue_new_bool(_value: bool) -> Box<SettingValue> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingValue::Bool(_value))
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

#[no_mangle]
pub extern "C" fn SettingValue_new_i64(_value: i64) -> Box<SettingValue> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingValue::I64(_value))
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

#[no_mangle]
pub extern "C" fn SettingValue_new_f64(_value: f64) -> Box<SettingValue> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingValue::F64(_value))
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

/// # Safety
/// TODO:
#[no_mangle]
pub unsafe extern "C" fn SettingValue_new_string(_value_ptr: *const u8) -> Box<SettingValue> {
    #[cfg(target_pointer_width = "64")]
    {
        Box::new(SettingValue::String(str(_value_ptr).into()))
    }
    #[cfg(not(target_pointer_width = "64"))]
    Box::new(())
}

#[no_mangle]
pub extern "C" fn SettingValue_drop(_: Box<SettingValue>) {}

#[no_mangle]
pub extern "C" fn SettingValue_get_type(_this: &SettingValue) -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        match _this {
            SettingValue::Map(_) => 1,
            SettingValue::List(_) => 2,
            SettingValue::Bool(_) => 3,
            SettingValue::I64(_) => 4,
            SettingValue::F64(_) => 5,
            SettingValue::String(_) => 6,
            _ => 0,
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    0
}

#[no_mangle]
pub extern "C" fn SettingValue_get_map(_this: &SettingValue) -> &SettingsMap {
    #[cfg(target_pointer_width = "64")]
    {
        match _this {
            SettingValue::Map(v) => v,
            _ => panic!("Wrong type"),
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Wrong type")
}

#[no_mangle]
pub extern "C" fn SettingValue_get_list(_this: &SettingValue) -> &SettingsList {
    #[cfg(target_pointer_width = "64")]
    {
        match _this {
            SettingValue::List(v) => v,
            _ => panic!("Wrong type"),
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Wrong type")
}

#[no_mangle]
pub extern "C" fn SettingValue_get_bool(_this: &SettingValue) -> bool {
    #[cfg(target_pointer_width = "64")]
    {
        match _this {
            SettingValue::Bool(v) => *v,
            _ => panic!("Wrong type"),
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Wrong type")
}

#[no_mangle]
pub extern "C" fn SettingValue_get_i64(_this: &SettingValue) -> i64 {
    #[cfg(target_pointer_width = "64")]
    {
        match _this {
            SettingValue::I64(v) => *v,
            _ => panic!("Wrong type"),
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Wrong type")
}

#[no_mangle]
pub extern "C" fn SettingValue_get_f64(_this: &SettingValue) -> f64 {
    #[cfg(target_pointer_width = "64")]
    {
        match _this {
            SettingValue::F64(v) => *v,
            _ => panic!("Wrong type"),
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Wrong type")
}

#[no_mangle]
pub extern "C" fn SettingValue_get_string(_this: &SettingValue) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        match _this {
            SettingValue::String(v) => output_str(v),
            _ => panic!("Wrong type"),
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Wrong type")
}
