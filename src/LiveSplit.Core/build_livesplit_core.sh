cd ../../lib/livesplit-core
FEATURES=parsing,image-shrinking
BINDINGS_DIR="$(pwd)/../../src/LiveSplit.Core/x64/target/bindings"
RUSTFLAGS="-C target-feature=+crt-static" cargo +stable-x86_64-pc-windows-msvc rustc --profile max-opt -p livesplit-core-capi --crate-type cdylib --no-default-features --features "$FEATURES" --target-dir ../../src/LiveSplit.Core/x64/target
cp ../../src/LiveSplit.Core/x64/target/max-opt/livesplit_core.dll ../../src/LiveSplit.Core/x64/.
RUSTFLAGS="-C target-feature=+crt-static" cargo +stable-i686-pc-windows-msvc rustc --profile max-opt -p livesplit-core-capi --crate-type cdylib --no-default-features --features "$FEATURES" --target-dir ../../src/LiveSplit.Core/x86/target
cp ../../src/LiveSplit.Core/x86/target/max-opt/livesplit_core.dll ../../src/LiveSplit.Core/x86/.
cd capi/bind_gen
cargo run -- --no-default-features --features "$FEATURES" --output-dir "$BINDINGS_DIR"
cp "$BINDINGS_DIR/LiveSplitCore.cs" ../../../../src/LiveSplit.Core/LiveSplitCore.g.cs
