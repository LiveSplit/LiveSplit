cd ../Libs/livesplit-core
RUSTFLAGS="-C target-feature=+crt-static" cargo +stable-x86_64-pc-windows-msvc rustc --release -p livesplit-core-capi --crate-type cdylib
cp target/release/livesplit_core.dll ../../LiveSplit.Core/x64/.
RUSTFLAGS="-C target-feature=+crt-static" cargo +stable-i686-pc-windows-msvc rustc --release -p livesplit-core-capi --crate-type cdylib
cp target/release/livesplit_core.dll ../../LiveSplit.Core/x86/.
cd capi/bind_gen
cargo run
cp ../bindings/LiveSplitCore.cs ../../../../LiveSplit.Core/.
