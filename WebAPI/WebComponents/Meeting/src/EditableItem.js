import React, { useEffect, useState } from "react";
import { Editor, EditorState, RichUtils } from 'draft-js';
import 'draft-js/dist/Draft.css';
import parse from 'html-react-parser';
import { stateFromHTML } from 'draft-js-import-html'
import { stateToHTML } from 'draft-js-export-html'
import { editorStyle } from './styles'

export default function EditableItem(props) {
    const { agendaItem, editableHTML, meetingId, language } = props
    const [userInput, setUserInput] = React.useState(false)
    const [editorState, setEditorState] = useState(EditorState.createEmpty())

    useEffect(() => {
        setEditorState(EditorState.createWithContent(stateFromHTML(editableHTML)))
    }, [])

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

    const repackHtml = (item) => {
        var div = document.createElement('div')
        div.innerHTML = agendaItem.html
        var newItem = div.querySelectorAll(".SisaltoSektio")[0]
        if (newItem) {
            var decisionDiv = div.querySelectorAll(".SisaltoPaatos")[0]
            if (!decisionDiv) {
                decisionDiv = document.createElement('div')
                decisionDiv.className = "SisaltoPaatos"
                decisionDiv.innerHTML = item
                div.appendChild(decisionDiv)
            } else {
                decisionDiv.innerHTML = item
            }
        } else {
            div.innerHTML = item
        }
        return div
    }

    const submitChanges = () => {
        const editedHtml = repackHtml(stateToHTML(editorState.getCurrentContent()));
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



