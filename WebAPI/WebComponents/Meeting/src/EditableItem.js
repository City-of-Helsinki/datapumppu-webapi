import { useLayoutEffect, useRef } from "react";
import { editableStyle } from './styles'

export default function EditableItem(props) {
    const { agendaItem, editableHTML, meetingId, language, onUpdated } = props
    const editorRef = useRef(null)

    useLayoutEffect(() => {
        editorRef.current.innerHTML = editableHTML
    }, [editableHTML])

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

    /*
        Implements support for ordered- and unordered lists in contentEditable div natively

        When user types "1. " on a new line, a new ordered list is created
        When user types "- " on a new line, a new unordered list is created
    */
   
    /*
        Inserts an <ol> or <ul> at the cursor position
        
        We do this specifically without using execCommand (which would otherwise be fine),
        so it only affects the block containing the trigger text and not surrounding content.

        To put it simply, we want to handle a case when there is normal text above when creating a new list like:

        Hello hello
        1. 

        The above is how it should look. If we use execCommand, the above would get formatted as follows, with the cursor moving to an inconvenient position as well:

        1. |Hello hello
    */
    const insertList = (listTag, triggerNode) => {
        const sel = window.getSelection()
        const list = document.createElement(listTag)
        const li = document.createElement('li')
        li.appendChild(document.createElement('br'))
        list.appendChild(li)

        triggerNode.textContent = ''

        if (triggerNode.parentElement === editorRef.current) {
            // Trigger text was a direct child of the editor — insert list at cursor
            const range = sel.getRangeAt(0)
            range.deleteContents()
            range.insertNode(list)
        } else {
            // Walk up to find the direct-child block of the editor so we don't
            // accidentally wrap surrounding lines into the new list
            let block = triggerNode.parentElement
            while (block.parentElement !== editorRef.current) block = block.parentElement
            if (block.textContent.trim() === '') {
                block.replaceWith(list)
            } else {
                block.after(list)
            }
        }

        const range = document.createRange()
        range.setStart(li, 0)
        range.collapse(true)
        sel.removeAllRanges()
        sel.addRange(range)
    }

    const handleKeyDown = (e) => {
        if (e.key === ' ') {
            const sel = window.getSelection()
            if (!sel.rangeCount) return
            const node = sel.anchorNode
            if (node.nodeType !== Node.TEXT_NODE) return
            const text = node.textContent.trim()

            // "1. " → ordered list
            // "- " or "* " → unordered list
            if (/^\d+\.$/.test(text) && sel.anchorOffset === node.textContent.length) {
                e.preventDefault()
                insertList('ol', node)
            } else if ((text === '-' || text === '*') && sel.anchorOffset === node.textContent.length) {
                e.preventDefault()
                insertList('ul', node)
            }
        }

        if (e.key === 'Backspace') {
            const sel = window.getSelection()
            if (!sel.rangeCount) return
            const node = sel.anchorNode
            const element = node.nodeType === Node.TEXT_NODE ? node.parentElement : node
            const li = element.closest('li')

            if (li && li.textContent.trim() === '') {
                // Empty li: remove it and either exit the list (if last item)
                // or move cursor to the end of the previous item (if in the middle)
                e.preventDefault()

                const list = li.closest('ol, ul')
                const prevLi = li.previousElementSibling
                const isLast = !li.nextElementSibling

                li.remove()

                if (isLast) {
                    const div = document.createElement('div')
                    div.appendChild(document.createElement('br'))
                    list.after(div)
                    const range = document.createRange()
                    range.setStart(div, 0)
                    range.collapse(true)
                    sel.removeAllRanges()
                    sel.addRange(range)
                } else if (prevLi) {
                    const range = document.createRange()
                    range.selectNodeContents(prevLi)
                    range.collapse(false)
                    sel.removeAllRanges()
                    sel.addRange(range)
                }
            } else if (li && sel.anchorOffset === 0) {
                /*
                    Cursor at start of non-empty li: pull it out as a plain div.
                    Without this, the browser does nothing (no previous li to merge into).

                    This is to specifically handle a case like:

                    1. |abc

                    If that list item is the first line in the editor, the list couldn't be removed without deleting "abc" first
                */
                e.preventDefault()

                const list = li.closest('ol, ul')
                const div = document.createElement('div')

                div.innerHTML = li.innerHTML
                li.remove()

                if (list.children.length === 0) {
                    list.replaceWith(div)
                } else {
                    list.before(div)
                }

                const range = document.createRange()
                range.setStart(div, 0)
                range.collapse(true)
                sel.removeAllRanges()
                sel.addRange(range)
            }
        }
    }

    const handleBlur = () => {
        const edited = editorRef.current.innerHTML
        const editedHtml = repackHtml(edited)
        const agendaPoint = agendaItem.agendaPoint

        fetch('#--API_URL--#/editor/edit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("userToken")
            },
            body: JSON.stringify({
                html: editedHtml,
                decision: edited,
                meetingId,
                agendaPoint,
                language
            })
        })

        onUpdated(editedHtml)
    }

    return (
        <div
            ref={editorRef}
            contentEditable
            suppressContentEditableWarning
            tabIndex="0"
            style={editableStyle}
            onKeyDown={handleKeyDown}
            onBlur={handleBlur}
        />
    )
}
