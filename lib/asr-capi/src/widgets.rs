use crate::settings_map::SettingsMap;

#[cfg(target_pointer_width = "64")]
use crate::{output_str, output_vec, setting_value::SettingValue};
#[cfg(target_pointer_width = "64")]
use livesplit_auto_splitting::settings::{FileFilter, Widget, WidgetKind};
#[cfg(target_pointer_width = "64")]
use std::sync::Arc;

#[cfg(target_pointer_width = "64")]
pub struct Widgets {
    pub inner: Arc<Vec<Widget>>,
}

#[cfg(not(target_pointer_width = "64"))]
pub type Widgets = ();

#[no_mangle]
pub extern "C" fn Widgets_drop(_: Box<Widgets>) {}

#[no_mangle]
pub extern "C" fn Widgets_len(_this: &Widgets) -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        _this.inner.len()
    }
    #[cfg(not(target_pointer_width = "64"))]
    0
}

#[no_mangle]
pub extern "C" fn Widgets_get_key(_this: &Widgets, _index: usize) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        output_str(&_this.inner[_index].key)
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_description(_this: &Widgets, _index: usize) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        output_str(&_this.inner[_index].description)
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_tooltip(_this: &Widgets, _index: usize) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        output_str(_this.inner[_index].tooltip.as_deref().unwrap_or_default())
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_type(_this: &Widgets, _index: usize) -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        match _this.inner[_index].kind {
            WidgetKind::Bool { .. } => 1,
            WidgetKind::Title { .. } => 2,
            WidgetKind::Choice { .. } => 3,
            WidgetKind::FileSelect { .. } => 4,
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_bool(
    _this: &Widgets,
    _index: usize,
    _settings_map: &SettingsMap,
) -> bool {
    #[cfg(target_pointer_width = "64")]
    {
        let setting = &_this.inner[_index];
        let WidgetKind::Bool { default_value } = setting.kind else {
            return false;
        };
        match _settings_map.get(&setting.key) {
            Some(SettingValue::Bool(stored)) => *stored,
            _ => default_value,
        }
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_choice_current_index(
    _this: &Widgets,
    _index: usize,
    _settings_map: &SettingsMap,
) -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        let setting = &_this.inner[_index];
        let WidgetKind::Choice {
            default_option_key,
            options,
        } = &setting.kind
        else {
            return 0;
        };
        let key = match _settings_map.get(&setting.key) {
            Some(SettingValue::String(stored)) => stored,
            _ => default_option_key,
        };
        options
            .iter()
            .position(|option| option.key == *key)
            .or_else(|| {
                options
                    .iter()
                    .position(|option| option.key == *default_option_key)
            })
            .unwrap_or_default()
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_choice_options_len(_this: &Widgets, _index: usize) -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        let setting = &_this.inner[_index];
        let WidgetKind::Choice { options, .. } = &setting.kind else {
            return 0;
        };
        options.len()
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_choice_option_key(
    _this: &Widgets,
    _index: usize,
    _option_index: usize,
) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        let setting = &_this.inner[_index];
        let WidgetKind::Choice { options, .. } = &setting.kind else {
            return output_str("");
        };
        output_str(&options[_option_index].key)
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_choice_option_description(
    _this: &Widgets,
    _index: usize,
    _option_index: usize,
) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        let setting = &_this.inner[_index];
        let WidgetKind::Choice { options, .. } = &setting.kind else {
            return output_str("");
        };
        output_str(&options[_option_index].description)
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_heading_level(_this: &Widgets, _index: usize) -> u32 {
    #[cfg(target_pointer_width = "64")]
    {
        let setting = &_this.inner[_index];
        let WidgetKind::Title { heading_level } = setting.kind else {
            return 0;
        };
        heading_level
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[no_mangle]
pub extern "C" fn Widgets_get_file_select_filter(_this: &Widgets, _index: usize) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        let setting = &_this.inner[_index];
        let WidgetKind::FileSelect { filters } = &setting.kind else {
            return output_str("");
        };
        output_vec(|o| {
            build_filter(filters, o);
        })
    }
    #[cfg(not(target_pointer_width = "64"))]
    panic!("Index out of bounds")
}

#[cfg(target_pointer_width = "64")]
fn build_filter(filters: &[FileFilter], output: &mut Vec<u8>) {
    for filter in filters.iter() {
        match filter {
            FileFilter::Name {
                description,
                pattern,
            } => {
                if pattern.contains([';', '|']) {
                    continue;
                }
                if !output.is_empty() {
                    output.push(b'|');
                }
                match &description {
                    Some(description) => {
                        output.extend(description.trim().bytes().filter(|b| *b != b'|'));
                        output.push(b'|');
                    }
                    None => {
                        let mime = pattern.split(' ').find_map(|pat| {
                            let (name, ext) = pat.rsplit_once('.')?;
                            if name != "*" {
                                return None;
                            }
                            if ext.contains('*') {
                                return None;
                            }
                            mime_guess::from_ext(ext).first()
                        });
                        if let Some(mime) = mime {
                            append_mime_desc(
                                mime.type_().as_str(),
                                mime.subtype().as_str(),
                                output,
                            );
                        } else {
                            let mut ext_count = 0;

                            let only_contains_extensions = pattern.split(' ').all(|pat| {
                                ext_count += 1;
                                let Some((name, ext)) = pat.rsplit_once('.') else {
                                    return false;
                                };
                                name == "*" && !ext.contains('*')
                            });

                            if only_contains_extensions {
                                let mut char_buf = [0; 4];

                                for (i, ext) in pattern
                                    .split(' ')
                                    .filter_map(|pat| {
                                        let Some((_, ext)) = pat.rsplit_once('.') else {
                                            return None;
                                        };
                                        Some(ext)
                                    })
                                    .enumerate()
                                {
                                    if i != 0 {
                                        output.extend_from_slice(if i + 1 != ext_count {
                                            b", "
                                        } else {
                                            b" or "
                                        });
                                    }

                                    for c in ext
                                        .chars()
                                        .flat_map(|c| c.to_uppercase())
                                        .filter(|c| *c != '|')
                                    {
                                        output.extend_from_slice(
                                            c.encode_utf8(&mut char_buf).as_bytes(),
                                        );
                                    }
                                }

                                output.extend_from_slice(b" files|");
                            } else {
                                output.extend_from_slice(pattern.trim().as_bytes());
                                output.push(b'|');
                            }
                        }
                    }
                }

                for (i, pattern) in pattern.split(' ').enumerate() {
                    if i != 0 {
                        output.push(b';');
                    }
                    output.extend_from_slice(pattern.as_bytes());
                }
            }
            FileFilter::MimeType(mime_type) => {
                let Some((top, sub)) = mime_type.split_once('/') else {
                    continue;
                };
                if top == "*" {
                    continue;
                }
                let Some(extensions) = mime_guess::get_extensions(top, sub) else {
                    continue;
                };

                if !output.is_empty() {
                    output.push(b'|');
                }

                append_mime_desc(top, sub, output);

                for (i, extension) in extensions.iter().enumerate() {
                    if i != 0 {
                        output.push(b';');
                    }
                    output.extend_from_slice(b"*.");
                    output.extend_from_slice(extension.as_bytes());
                }
            }
        }
    }

    if !output.is_empty() {
        output.push(b'|');
    }
    output.extend_from_slice(b"All files (*.*)|*.*");
}

#[cfg(target_pointer_width = "64")]
fn append_mime_desc(top: &str, sub: &str, output: &mut Vec<u8>) {
    let mut char_buf = [0; 4];

    if sub != "*" {
        // Strip vendor and x- prefixes

        let sub = sub
            .strip_prefix("vnd.")
            .unwrap_or(sub)
            .strip_prefix("x-")
            .unwrap_or(sub);

        // Capitalize the first letter

        let mut chars = sub.chars();
        if let Some(c) = chars
            .by_ref()
            .map(|c| match c {
                '-' | '.' | '+' | '|' => ' ',
                _ => c,
            })
            .next()
        {
            for c in c.to_uppercase() {
                output.extend_from_slice(c.encode_utf8(&mut char_buf).as_bytes());
            }
        }

        // Only capitalize chunks of the rest that are 4 characters or less as a
        // heuristic to detect acronyms

        let rem = chars.as_str();
        for (i, piece) in rem.split(&['-', '.', '+', '|', ' ']).enumerate() {
            if i != 0 {
                output.push(b' ');
            }
            if piece.len() <= 4 - (i == 0) as usize {
                for c in piece.chars() {
                    for c in c.to_uppercase() {
                        output.extend_from_slice(c.encode_utf8(&mut char_buf).as_bytes());
                    }
                }
            } else {
                output.extend_from_slice(piece.as_bytes());
            }
        }

        output.push(b' ');
    }

    let mut chars = top.chars();
    if sub == "*" {
        if let Some(c) = chars.by_ref().find(|c| *c != '|') {
            for c in c.to_uppercase() {
                output.extend_from_slice(c.encode_utf8(&mut char_buf).as_bytes());
            }
        }
    }
    output.extend(chars.as_str().bytes().filter(|b| *b != b'|'));
    output.extend_from_slice(if top == "image" { b"s|" } else { b" files|" });
}
