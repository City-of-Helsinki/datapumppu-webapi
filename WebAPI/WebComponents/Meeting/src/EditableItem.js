import React, { useEffect, useState } from "react";
import { Editor, EditorState, RichUtils } from 'draft-js';
import 'draft-js/dist/Draft.css';
import parse from 'html-react-parser';
import { stateFromHTML } from 'draft-js-import-html'
import { stateToHTML } from 'draft-js-export-html'
import { editorStyle } from './styles'
import { useTranslation } from 'react-i18next';

export default function EditableItem(props) {
    const { agendaItem, meetingId, language } = props
    const [userInput, setUserInput] = React.useState(false)
    const [editorState, setEditorState] = useState(EditorState.createEmpty());
    const { t } = useTranslation();

    useEffect(() => {
        const wrapUnwrapped = () => {
            var div = document.createElement('div')
            div.innerHTML = agendaItem.html
            var nodes = div.childNodes
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].nodeName == "H4") {
                    div.removeChild(nodes[i])
                } else if (nodes[i].nodeType == 3) {
                    var p = document.createElement('p');
                    p.textContent = nodes[i].textContent;
                    div.replaceChild(p, nodes[i])
                }
            }
            setEditorState(EditorState.createWithContent(stateFromHTML(div.innerHTML)))
        }
        wrapUnwrapped()
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

    const submitChanges = () => {
        const editedHtml = stateToHTML(editorState.getCurrentContent());
        const wrappedWithHeader = "<html><body><h4>" + t("Decision") + "</h4>" + editedHtml + "</body></html>"

        const agendaPoint = agendaItem.agendaPoint
        const request = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("userToken")
            },
            body: JSON.stringify({
                html: wrappedWithHeader,
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
            <h4>{t("Decision")}</h4>
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



