function Word.Application'app $name

jaw_path=_s.expandpath(name)

Word.Documents docs=app.Documents
jaw_doc=docs.Add
jaw_rng=jaw_doc.Range
