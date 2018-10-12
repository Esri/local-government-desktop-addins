
- Create a stored procedure in your database using a name such as GETNEXTVALUE
- Create a public synonym so it can be accessed by all editors
  - ex format for Oracle: create public synonym sequence_name for yourSchema.sequence_name;
- Update the configuration file and provide the name of synonym
