import React, { useState } from "react";
import { Editor, EditorState, RichUtils } from 'draft-js';
import 'draft-js/dist/Draft.css';
import parse from 'html-react-parser';
import { stateFromHTML } from 'draft-js-import-html'
import { stateToHTML } from 'draft-js-export-html'
import { editorStyle } from './styles'

export default function EditableItem(props) {
    const { agendaItem, meetingId, language } = props
    const [userInput, setUserInput] = React.useState(false)
    const [editorState, setEditorState] = useState(EditorState.createWithContent(stateFromHTML(agendaItem.html)));

    const onChange = editorState => {
        setEditorState(editorState)
    };

    const handleKeyCommand = command => {
        const newState = RichUtils.handleKeyCommand(
            editorState,
            command
        );
        if (newState) {
            onChange(newState);
            return "handled";
        }
        return "not-handled";
    };

    const submitChanges = () => {
        const editedHtml = stateToHTML(editorState.getCurrentContent());
        const agendaPoint = agendaItem.agendaPoint
        const request = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("userToken")
            },
            body: JSON.stringify({
                html: editedHtml,
                meetingId,
                agendaPoint,
                language
            })
        }
        fetch('#--API_URL--#/editor/edit', request)
        agendaItem.html = editedHtml

        setUserInput(false)
    }


    return (
        <div>
            <div tabIndex="0" onFocus={() => setUserInput(true)} >
                {
                    userInput ?
                        <div style={editorStyle} onBlur={() => submitChanges()}>
                            <Editor editorState={editorState} onChange={onChange} handleKeyCommand={handleKeyCommand} />
                        </div> :
                        <div>
                            {parse(stateToHTML(editorState.getCurrentContent()))}
                        </div>
                }
            </div>
        </div>
    );
}



