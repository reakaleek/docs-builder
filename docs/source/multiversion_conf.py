# -- Options for sphinx-multiversion -----------------------------------------
# https://holzhaus.github.io/sphinx-multiversion/master/configuration.html

# Whitelist pattern for tags (set to None to ignore all tags)
# smv_tag_whitelist = None                      # Ignore all tags

# Whitelist pattern for branches (set to None to ignore all branches)
# smv_branch_whitelist = r'^\d+\.\d+$'          # Include branches like "2.1"
smv_branch_whitelist = r'^.*$'                # Include all branches

smv_remote_whitelist = None                   # Only use local branches
#smv_remote_whitelist = r'^.*$'                # Use branches from all remotes

# Pattern for released versions
# smv_released_pattern = r'^refs/heads/\d+\.\d+$'    # Branches like "2.1"

# Format for versioned output directories inside the build directory
#smv_outputdir_format = '{ref.name}'

# Determines whether remote or local git branches/tags are preferred if their output dirs conflict
#smv_prefer_remote_refs = False