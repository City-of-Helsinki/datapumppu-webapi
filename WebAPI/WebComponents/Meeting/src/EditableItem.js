import React, { useEffect, useState } from "react";
import { Editor, EditorState, RichUtils } from 'draft-js';
import 'draft-js/dist/Draft.css';
import parse from 'html-react-parser';
import { stateFromHTML } from 'draft-js-import-html'
import { stateToHTML } from 'draft-js-export-html'
import { editorStyle, editableStyle } from './styles'

export default function EditableItem(props) {
    const { agendaItem, editableHTML, meetingId, language, onUpdated } = props
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
        var newDiv = document.createElement('div')
        div.innerHTML = agendaItem.html
        var content = div.querySelectorAll(".SisaltoSektio")[0]
        if (content) {
            const editableDiv = document.createElement('div')
            editableDiv.innerHTML = item
            newDiv.appendChild(editableDiv)
            newDiv.appendChild(content)
        } else {
            newDiv.innerHTML = item
        }
        return newDiv.innerHTML
    }

    const submitChanges = () => {
        const editedPart = editorState.getCurrentContent()
        const editedHtml = repackHtml(stateToHTML(editedPart))
        const agendaPoint = agendaItem.agendaPoint
        const request = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("userToken")
            },
            body: JSON.stringify({
                html: editedHtml,
                decision: editedPart,
                meetingId,
                agendaPoint,
                language
            })
        }
        fetch('#--API_URL--#/editor/edit', request)
        onUpdated(editedHtml)
        setUserInput(false)
    }

    return (
        <div>
            <div  tabIndex="0" onFocus={() => setUserInput(true)} >
                {
                    userInput ?
                        <div style={editorStyle} onBlur={() => submitChanges()}>
                            <Editor editorState={editorState} onChange={onChange} handleKeyCommand={handleKeyCommand} />
                        </div> :
                        <div style={editableStyle}>
                            {parse(stateToHTML(editorState.getCurrentContent()))}
                        </div>
                }
            </div>
        </div>
    );
}



