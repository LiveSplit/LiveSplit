#[cfg(target_pointer_width = "64")]
use {
    livesplit_auto_splitting::{time, wasi_path, Timer, TimerState},
    std::{cell::RefCell, ffi::CStr, fmt, path::Path},
};

mod runtime;
mod setting_value;
mod settings_list;
mod settings_map;
mod widgets;

#[cfg(target_pointer_width = "64")]
thread_local! {
    static OUTPUT_VEC: RefCell<Vec<u8>>  = RefCell::new(Vec::new());
}

#[cfg(target_pointer_width = "64")]
fn output_vec<F>(f: F) -> *const u8
where
    F: FnOnce(&mut Vec<u8>),
{
    OUTPUT_VEC.with_borrow_mut(|output| {
        output.clear();
        f(output);
        output.push(0);
        output.as_ptr()
    })
}

#[cfg(target_pointer_width = "64")]
fn output_str(s: &str) -> *const u8 {
    output_vec(|o| {
        o.extend_from_slice(s.as_bytes());
    })
}

#[cfg(target_pointer_width = "64")]
unsafe fn str(s: *const u8) -> &'static str {
    if s.is_null() {
        ""
    } else {
        let bytes = CStr::from_ptr(s.cast()).to_bytes();
        std::str::from_utf8_unchecked(bytes)
    }
}

#[cfg(target_pointer_width = "64")]
pub struct CTimer {
    state: unsafe extern "C" fn() -> i32,
    start: unsafe extern "C" fn(),
    split: unsafe extern "C" fn(),
    skip_split: unsafe extern "C" fn(),
    undo_split: unsafe extern "C" fn(),
    reset: unsafe extern "C" fn(),
    set_game_time: unsafe extern "C" fn(i64),
    pause_game_time: unsafe extern "C" fn(),
    resume_game_time: unsafe extern "C" fn(),
    log: unsafe extern "C" fn(*const u8, usize),
}

#[cfg(target_pointer_width = "64")]
impl Timer for CTimer {
    fn state(&self) -> TimerState {
        match unsafe { (self.state)() } {
            1 => TimerState::Running,
            2 => TimerState::Paused,
            3 => TimerState::Ended,
            _ => TimerState::NotRunning,
        }
    }

    fn start(&mut self) {
        unsafe { (self.start)() }
    }

    fn split(&mut self) {
        unsafe { (self.split)() }
    }

    fn skip_split(&mut self) {
        unsafe { (self.skip_split)() }
    }

    fn undo_split(&mut self) {
        unsafe { (self.undo_split)() }
    }

    fn reset(&mut self) {
        unsafe { (self.reset)() }
    }

    fn set_game_time(&mut self, time: time::Duration) {
        const TICKS_PER_SEC: i64 = 10_000_000;
        const NANOS_PER_SEC: i64 = 1_000_000_000;
        const NANOS_PER_TICK: i64 = NANOS_PER_SEC / TICKS_PER_SEC;

        let (secs, nanos) = (time.whole_seconds(), time.subsec_nanoseconds());
        let ticks = secs * TICKS_PER_SEC + nanos as i64 / NANOS_PER_TICK;
        unsafe { (self.set_game_time)(ticks) }
    }

    fn pause_game_time(&mut self) {
        unsafe { (self.pause_game_time)() }
    }

    fn resume_game_time(&mut self) {
        unsafe { (self.resume_game_time)() }
    }

    fn set_variable(&mut self, _: &str, _: &str) {}

    fn log(&mut self, message: fmt::Arguments<'_>) {
        log(self.log, message);
    }
}

#[cfg(target_pointer_width = "64")]
fn log(log_fn: unsafe extern "C" fn(*const u8, usize), message: fmt::Arguments<'_>) {
    let mut owned;
    let message = match message.as_str() {
        Some(m) => m,
        None => {
            owned = smallstr::SmallString::<[u8; 4 << 10]>::new();
            use std::fmt::Write;
            let _ = write!(owned, "{message}");
            &owned
        }
    };
    unsafe { log_fn(message.as_ptr(), message.len()) }
}

/// Returns the byte length of the last nul-terminated string returned on the
/// current thread. The length excludes the nul-terminator.
#[no_mangle]
pub extern "C" fn get_buf_len() -> usize {
    #[cfg(target_pointer_width = "64")]
    {
        OUTPUT_VEC.with(|v| v.borrow().len() - 1)
    }
    #[cfg(not(target_pointer_width = "64"))]
    0
}

/// Translates `original_path` into a path that is accessible through the WASI
/// file system, so a Windows path of `C:\foo\bar.exe` would be returned as
/// `/mnt/c/foo/bar.exe`.
///
/// # Safety
/// `original_path` must be a valid nul-terminated UTF-8 string.
#[no_mangle]
pub unsafe extern "C" fn path_to_wasi(_original_path: *const u8) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        let wasi = wasi_path::from_native(Path::new(str(_original_path))).unwrap_or_default();
        output_str(&wasi)
    }
    #[cfg(not(target_pointer_width = "64"))]
    "\0".as_ptr()
}

/// Translates from a path accessible through the WASI file system to a path
/// accessible outside that, so a WASI path of `/mnt/c/foo/bar.exe` would be
/// translated on Windows to `C:\foo\bar.exe`.
///
/// # Safety
/// `wasi_path` must be a valid nul-terminated UTF-8 string.
#[no_mangle]
pub unsafe extern "C" fn wasi_to_path(_wasi_path: *const u8) -> *const u8 {
    #[cfg(target_pointer_width = "64")]
    {
        let path = wasi_path::to_native(str(_wasi_path)).unwrap_or_default();
        output_str(path.to_str().unwrap_or_default())
    }
    #[cfg(not(target_pointer_width = "64"))]
    "\0".as_ptr()
}
