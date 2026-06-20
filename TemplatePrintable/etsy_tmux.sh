#!/bin/bash

TARGET_DIR="$HOME/z_ob/etsy/TemplatePrintable"
ARGUMENT=$1

tmux new-session -d -s etsy -c "$TARGET_DIR"

tmux split-window -h -l 6 -t etsy -c "$TARGET_DIR"

if [ -n "$ARGUMENT" ]; then
    if ls "$TARGET_DIR/output/$ARGUMENT/"*.pdf &> /dev/null; then
        tmux send-keys -t etsy:0.1 "evince output/$ARGUMENT/*.pdf &> /dev/null &" C-m
        tmux send-keys -t etsy:0.1 "sleep 1" C-m
        tmux send-keys -t etsy:0.1 "source env.sh" C-m
        tmux send-keys -t etsy:0.1 "sleep 1" C-m
        tmux send-keys -t etsy:0.1 "dwr" C-m
    else
        tmux send-keys -t etsy:0.1 "echo 'ERROR: No PDF files found in output/$ARGUMENT/'" C-m
    fi
fi

tmux attach-session -t etsy
